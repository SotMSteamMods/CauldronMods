using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCharacterCardController : HeroCharacterCardController
    {
        private PyreTurnTakerController PyreTTC
        {
            get
            {
                if (TurnTakerControllerWithoutReplacements is PyreTurnTakerController pyreTTC)
                {
                    return pyreTTC;
                }
                return null;
            }
        }
        protected PyreUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            if(TurnTakerController is PyreTurnTakerController ttc)
            {
                ttc.MoveMarkersToSide();
            }
        }

        protected bool IsIrradiated(Card c)
        {
            if(c != null && c.IsInHand)
            {
                return c.NextToLocation.Cards.Any((Card nextTo) => nextTo.Identifier == "IrradiatedMarker");
            }
            return false;
        }

        protected IEnumerator IrradiateCard(Card cardToIrradiate)
        {
            var marker = TurnTaker.GetAllCards(realCardsOnly: false).Where((Card c) => !c.IsRealCard && c.Location.IsOffToTheSide).FirstOrDefault();
            if(marker != null && cardToIrradiate.Location.IsHand)
            {
                IEnumerator coroutine = GameController.MoveCard(DecisionMaker, marker, cardToIrradiate.NextToLocation, doesNotEnterPlay: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                var irradiateEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), $"{cardToIrradiate.Title} is irradiated.", new TriggerType[] { TriggerType.Hidden }, Card);
                irradiateEffect.CardMovedExpiryCriteria.Card = cardToIrradiate;

                coroutine = AddStatusEffect(irradiateEffect, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(PyreTTC != null)
                {
                    PyreTTC.AddIrradiatedSpecialString(cardToIrradiate);
                }
            }
            yield break;
        }
        protected IEnumerator ClearIrradiation(Card c)
        {
            yield break;
        }
    }
}
