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
            var viableHeroes = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(c => !c.IsIrradiated()) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
            var decision = new SelectTurnTakerDecision(GameController, DecisionMaker, viableHeroes, SelectionType.Custom, cardSource: GetCardSource());
            CurrentMode = CustomMode.PlayerToIrradiate;
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
            var controller = FindCardController(selectedCard);
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

                //select the power
                var indexOfPower = -1;
                if (selectedCard.NumberOfPowers == 1)
                {
                    var storedYesNo = new List<YesNoCardDecision>();
                    coroutine = GameController.MakeYesNoCardDecision(heroTTC, SelectionType.UsePower, selectedCard, storedResults: storedYesNo, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    if (DidPlayerAnswerYes(storedYesNo))
                    {
                        indexOfPower = 0;
                    }
                }
                else if (selectedCard.NumberOfPowers > 1)
                {
                    var powerChoices = new List<Function>();
                    for (int i = 0; i < selectedCard.NumberOfPowers; i++)
                    {
                        powerChoices.Add(new Function(heroTTC, $"Use power {i + 1}: {controller.Card.CurrentPowers.ElementAt(i)}", SelectionType.UsePower, DoNothing));
                    }
                    var selectPower = new SelectFunctionDecision(GameController, heroTTC, powerChoices, true, associatedCards: new List<Card> { selectedCard }, cardSource: GetCardSource());
                    coroutine = GameController.SelectAndPerformFunction(selectPower);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    if (selectPower.Index != null)
                    {
                        indexOfPower = selectPower.Index.Value;
                    }
                }

                if (indexOfPower == -1)
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


                //use the power
                coroutine = UsePowerOnOtherCard(selectedCard, indexOfPower);
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
            else if (controller is Malichae.DjinnOngoingController djinn)
            {
                if (!GameController.CanUsePowers(heroTTC, GetCardSource()))
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

                //djinn prep
                var pwr = djinn.GetGrantedPower(djinn, djinn.FindBaseDjinn().First());
                var cs = djinn.GetCardSource();
                djinn.AddAssociatedCardSource(cs);

                //power use in hand effect
                bool isOnList = GameController.IsInCardControllerList(selectedCard, CardControllerListType.CanCauseDamageOutOfPlay);
                GameController.AddInhibitorException(djinn, (GameAction ga) => ga != null && ga.CardSource != null && ga.CardSource.Card == cs.Card);
                if (!isOnList)
                {
                    GameController.AddCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay, djinn);
                }
                CardUsingPower = selectedCard;

                //use power
                coroutine = GameController.UsePower(pwr, true, DecisionMaker, cs);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //cleanup power use
                if (!isOnList)
                {
                    GameController.RemoveCardControllerFromList(CardControllerListType.CanCauseDamageOutOfPlay, djinn);
                }
                GameController.RemoveInhibitorException(djinn);

                //clean up djinn
                djinn.RemoveAssociatedCardSource(cs);

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
