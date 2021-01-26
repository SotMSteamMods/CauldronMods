using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCardController : CardController
    {
        protected PyreUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsIrradiated(Card c)
        {
            if (c != null && (c.IsInHand || c.Location.IsRevealed))
            {
                return IsByIrradiationMarker(c);
            }
            return false;
        }
        protected bool IsByIrradiationMarker(Card c)
        {
            if (c != null)
            {
                return c.NextToLocation.Cards.Any((Card nextTo) => nextTo.Identifier == "IrradiatedMarker");
            }
            return false;
        }

        protected IEnumerator IrradiateCard(Card cardToIrradiate)
        {
            var marker = TurnTaker.GetAllCards(realCardsOnly: false).Where((Card c) => !c.IsRealCard && c.Location.IsOffToTheSide).FirstOrDefault();
            if (marker != null && cardToIrradiate.Location.IsHand)
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

                var irradiateEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), $"{cardToIrradiate.Title} is irradiated until it leaves {cardToIrradiate.Location.GetFriendlyName()}.", new TriggerType[] { TriggerType.Hidden }, Card);
                irradiateEffect.CardMovedExpiryCriteria.Card = cardToIrradiate;

                coroutine = AddStatusEffect(irradiateEffect, true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                /*
                if(PyreTTC != null)
                {
                    PyreTTC.AddIrradiatedSpecialString(cardToIrradiate);
                }
                */
            }
            yield break;
        }
    }
}
