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
        }

        public override void AddTriggers()
        {
            //"When Rogue Fission Cascade would enter play, instead discard it and draw a card. Then put a Cascade card from your trash on top of your deck and destroy this card.",
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.Identifier == "RogueFissionCascade", PreventCascadeResponse, TriggerType.CancelAction, TriggerTiming.Before);
        }
        private IEnumerator PreventCascadeResponse(CardEntersPlayAction cep)
        {
            //Instead,
            IEnumerator coroutine = CancelAction(cep);
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
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.CardFromHand, tt => SelectAndIrradiateCardInHand(tt, numCardsInHand), allowAutoDecide: true, cardSource: GetCardSource());
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
