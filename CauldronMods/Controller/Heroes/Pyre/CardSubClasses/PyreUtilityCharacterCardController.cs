using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCharacterCardController : HeroCharacterCardController
    {
        private const string IrradiationEffectFunction = "FakeIrradiationStatusEffectFunction";
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
        /*
        public override void AddTriggers()
        {
            base.AddTriggers();
            AddTrigger((MoveCardAction mc) => IsByIrradiationMarker(mc.CardToMove) && (mc.Origin.IsHand || mc.Origin.IsRevealed) && !(mc.Destination.IsHand || mc.Destination.IsRevealed), mc => ClearIrradiation(mc.CardToMove), TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
        }
        */
        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            AddTrigger((MoveCardAction mc) => IsByIrradiationMarker(mc.CardToMove) && (mc.Origin.IsHand || mc.Origin.IsRevealed) && !(mc.Destination.IsHand || mc.Destination.IsRevealed), mc => ClearIrradiation(mc.CardToMove), TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
            AddTrigger((PlayCardAction pc) => IsByIrradiationMarker(pc.CardToPlay), pc => ClearIrradiation(pc.CardToPlay), TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
            AddTrigger((BulkMoveCardsAction bmc) => !(bmc.Destination.IsHand || bmc.Destination.IsRevealed) && bmc.CardsToMove.Any(c => IsByIrradiationMarker(c)), CleanUpBulkIrradiated, TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
        }

        protected bool IsIrradiated(Card c)
        {
            if(c != null && c.IsInHand)
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

        protected bool IsCascade(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "cascade");
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

                var irradiateEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, IrradiationEffectFunction, $"{cardToIrradiate.Title} is irradiated until it leaves {cardToIrradiate.Location.GetFriendlyName()}.", new TriggerType[] { TriggerType.Hidden }, cardToIrradiate);
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
        protected IEnumerator ClearIrradiation(Card card)
        {
            //Log.Debug($"ClearIrradiation called on {card.Title}");
            var marks = card?.NextToLocation.Cards.Where((Card c) => !c.IsRealCard && c.Identifier == "IrradiatedMarker");
            if(marks != null && marks.Any())
            {
                IEnumerator coroutine = BulkMoveCard(DecisionMaker, marks, TurnTaker.OffToTheSide, false, false, DecisionMaker, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            var irradiationEffects = GameController.StatusEffectControllers.Where((StatusEffectController sec) => sec.StatusEffect is OnPhaseChangeStatusEffect opc && (opc.MethodToExecute == IrradiationEffectFunction && opc.CardMovedExpiryCriteria.Card == card)).Select(sec => sec.StatusEffect).ToList();
            foreach(StatusEffect effect in irradiationEffects)
            {
                IEnumerator coroutine = GameController.ExpireStatusEffect(effect, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        protected IEnumerator CleanUpBulkIrradiated(BulkMoveCardsAction bmc)
        {
            foreach(Card c in bmc.CardsToMove)
            {
                if(IsByIrradiationMarker(c))
                {
                    IEnumerator coroutine = ClearIrradiation(c);
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
            yield break;
        }
    }
}
