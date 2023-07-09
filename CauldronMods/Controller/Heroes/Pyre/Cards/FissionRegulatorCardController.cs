using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class FissionRegulatorCardController : PyreUtilityCardController
    {
        public FissionRegulatorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(() => TurnTaker.Deck, new LinqCardCriteria((Card c) => IsCascade(c), "cascade"));
        }

        public readonly string RogueFissionCascadeIdentifier = "RogueFissionCascade";

        public override void AddTriggers()
        {
            //"When Rogue Fission Cascade would enter play, instead discard it and draw a card. Then put a Cascade card from your trash on top of your deck and destroy this card.",
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.Identifier == RogueFissionCascadeIdentifier, PreventCascadeResponse, TriggerType.CancelAction, TriggerTiming.Before);
        }
        private IEnumerator PreventCascadeResponse(CardEntersPlayAction cep)
        {
            //Instead,
            IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} delays a catastrophic cascade!", Priority.High, GetCardSource(), associatedCards: new Card[] { cep.CardEnteringPlay }, true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = CancelAction(cep);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //discard it...
            coroutine = GameController.MoveCard(DecisionMaker, cep.CardEnteringPlay, cep.CardEnteringPlay.NativeTrash, isDiscard: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //and draw a card.
            coroutine = DrawCard();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Then put a Cascade card from your trash on top of your deck...
            var cascadeInTrash = FindCardsWhere((Card c) => c.Location == TurnTaker.Trash && IsCascade(c)).FirstOrDefault();
            if(cascadeInTrash != null)
            {
                coroutine = GameController.SendMessageAction($"{Card.Title} moves {cascadeInTrash.Title} to the top of {TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource(), new Card[] { cascadeInTrash });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.MoveCard(DecisionMaker, cascadeInTrash, DecisionMaker.TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //and destroy this card.
            coroutine = DestroyThisCardResponse(cep);
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
        public override IEnumerator UsePower(int index = 0)
        {
            int numCardsInHand = GetPowerNumeral(0, 1);
            //"Each player selects 1 non-{PyreIrradiate} card in their hand. {PyreIrradiate} those cards until they leave their hands."
            CurrentMode = CustomMode.PlayerToIrradiate;
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.Custom, tt => SelectAndIrradiateCardInHand(tt, numCardsInHand), allowAutoDecide: true, cardSource: GetCardSource());
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

        private IEnumerator SelectAndIrradiateCardInHand(TurnTaker tt, int toIrradiate)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            return SelectAndIrradiateCardsInHand(heroTTC, tt, toIrradiate, toIrradiate);
        }
    }
}
