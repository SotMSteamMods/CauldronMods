using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class CherenkovDriveCardController : PyreUtilityCardController
    {
        private Card CardUsingPower;
        private bool? TriedSelfDestruct;
        public CherenkovDriveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "Powers which cause multiple instances of the same thing (for example card draws, or damage to multiple targets) may stop working after the first instance, as they notice the card is out of play.");
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, 1 player may select 1 non-{PyreIrradiate} card in their hand. {PyreIrradiate} that card it until it leaves their hand. Then, they may use a power on that card.",
            //"If that power destroys that card, discard it instead."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnIrradiateResponse, TriggerType.UsePower);
        }

        private IEnumerator EndOfTurnIrradiateResponse(PhaseChangeAction _)
        {
            var viableHeroes = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(c => !IsIrradiated(c)) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
            var decision = new SelectTurnTakerDecision(GameController, DecisionMaker, viableHeroes, SelectionType.CardFromHand, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(decision, IrradiateCardInHandAndMaybeUsePower);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator IrradiateCardInHandAndMaybeUsePower(TurnTaker tt)
        {
            //1 player may select 1 non-{PyreIrradiate} card in their hand. {PyreIrradiate} that card it until it leaves their hand.
            var storedCard = new List<SelectCardDecision>();
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            IEnumerator coroutine = SelectAndIrradiateCardsInHand(heroTTC, tt, 1, 0, storedResults: storedCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(storedCard))
            {
                yield break;
            }

            // Then, they may use a power on that card.
            var selectedCard = GetSelectedCard(storedCard);
            if (selectedCard.HasPowers)
            {

                if(!GameController.CanUsePowers(heroTTC, GetCardSource()))
                {
                    coroutine = GameController.SendMessageAction($"{Card.Title} would allow {tt.Name} to use a power on {selectedCard.Title}, but they cannot currently use powers.", Priority.High, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    yield break;
                }

                var controller = FindCardController(selectedCard);

                var powersOnCard = new List<Power>();
                for (int i = 0; i < selectedCard.NumberOfPowers; i++)
                {
                    powersOnCard.Add(new Power(heroTTC, controller, controller.Card.CurrentPowers.ElementAt(i), controller.UsePower(i), i, null, controller.GetCardSource()));
                }

                //select the power
                var powerDecision = new UsePowerDecision(GameController, heroTTC, powersOnCard,true, cardSource: GetCardSource());
                coroutine = GameController.MakeDecisionAction(powerDecision);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(powerDecision.SelectedPower == null)
                {
                    yield break;
                }

                //if they picked one, prep the card to do stuff
                bool wasOnList = GameController.IsInCardControllerList(selectedCard, CardControllerListType.CanCauseDamageOutOfPlay);
                GameController.AddInhibitorException(controller, (GameAction ga) => ga != null && ga.CardSource != null && ga.CardSource.Card == selectedCard);
                if (!wasOnList)
                {
                    GameController.AddCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay, controller);
                }
                CardUsingPower = selectedCard;


                /*
                 * slightly simpler way to do this?
                coroutine = GameController.SelectAndUsePower(heroTTC, true, (Power p) => p.CardController == controller, allowAnyHeroPower: true, allowOutOfPlayPower: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                */

                //use the power
                coroutine = GameController.UsePower(powerDecision.SelectedPower, heroUsingPower: heroTTC, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //clean up the prepwork
                if (!wasOnList)
                {
                    GameController.RemoveCardControllerFromList(CardControllerListType.CanCauseDamageOutOfPlay, controller);
                }
                GameController.RemoveInhibitorException(controller);
                CardUsingPower = null;

                //"If that power destroys that card, discard it instead."
                if (TriedSelfDestruct == true)
                {
                    TriedSelfDestruct = null;
                    coroutine = GameController.DiscardCard(heroTTC, selectedCard, null, TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            CardUsingPower = null;
            TriedSelfDestruct = null;
            yield break;
        }

        public override bool AskIfCardIsIndestructible(Card c)
        {
            if(CardUsingPower == c)
            {
                //this is done to check whether the power destroys the card it is on
                //GameController.DestroyCard checks if the card it's aiming at is out-of-play before making a DestroyCardAction,
                //but it will do indestructibility checks before it does an in-play check.
                TriedSelfDestruct = true;
            }
            return false;
        }
    }
}
