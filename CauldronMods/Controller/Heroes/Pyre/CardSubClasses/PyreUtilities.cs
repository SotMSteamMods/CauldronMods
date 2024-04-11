using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Pyre
{

    public static class PyreExtensionMethods
    {
        private const string IrradiationEffectFunction = "FakeIrradiationStatusEffectFunction";
        public const string CascadeKeyword = "cascade";
        public const string Irradiated = "{Rad}";

        public static void ShowIrradiatedCardsInHands(this CardController co, SpecialStringMaker maker)
        {
            foreach (HeroTurnTaker hero in co.GameController.Game.HeroTurnTakers)
            {
                maker.ShowListOfCardsAtLocation(hero.Hand, new LinqCardCriteria((Card c) => c.IsIrradiated(), Irradiated));
            }
        }
        public static void ShowIrradiatedCount(this CardController co, SpecialStringMaker maker, bool reverse = false)
        {
            string descriptor = reverse ? $"non-{Irradiated}" : Irradiated;
            maker.ShowNumberOfCardsAtLocations(() => co.GameController.AllHeroes.Where(htt => !htt.IsIncapacitatedOrOutOfGame).Select(htt => htt.Hand), new LinqCardCriteria((Card c) => c.IsIrradiated() != reverse, descriptor));
        }

        public static bool IsIrradiated(this Card c)
        {
            if (c != null && (c.IsInHand || c.Location.IsRevealed))
            {
                return c.IsByIrradiationMarker();
            }
            return false;
        }

        public static bool IsByIrradiationMarker(this Card c)
        {
            if (c != null)
            {
                return c.NextToLocation.Cards.Any((Card nextTo) => nextTo.IsIrradiatedCard());
            }
            return false;
        }

        public static bool IsCascade(this GameController g, Card c)
        {
            return g.DoesCardContainKeyword(c, CascadeKeyword);
        }

        public static bool IsIrradiatedCard(this Card c)
        {
            return c.Identifier == "IrradiatedMarker" && c.Definition.ParentDeck.Identifier == "Pyre";
        }

        public static IEnumerator IrradiateCard(this CardController co, Card cardToIrradiate)
        {
            if (cardToIrradiate.IsIrradiated() || ! cardToIrradiate.Location.IsHand)
            {
                yield break;
            }

            var marker = co.TurnTakerControllerWithoutReplacements.TurnTaker.GetAllCards(realCardsOnly: false).Where((Card c) => c.IsIrradiatedCard() && c.Location.IsOffToTheSide).FirstOrDefault();
            if (marker == null)
            {
                // Need to synthesise a card.
                var anyIrradiated = co.GameController.FindCardsWhere(c => c.IsIrradiatedCard(), realCardsOnly: false).First();
                if (anyIrradiated == null)
                {
                    throw new InvalidOperationException("Couldn't find any IrradiatedMarker cards");
                }

                marker = new Card(anyIrradiated.Definition, co.TurnTakerControllerWithoutReplacements.TurnTaker, 0);
                co.TurnTakerControllerWithoutReplacements.TurnTaker.OffToTheSide.AddCard(marker);

                var newController = CardControllerFactory.CreateInstance(marker, co.TurnTakerControllerWithoutReplacements);
                co.TurnTakerControllerWithoutReplacements.AddCardController(newController);
            }

            IEnumerator coroutine = co.GameController.MoveCard(co.DecisionMaker, marker, cardToIrradiate.NextToLocation, doesNotEnterPlay: true, cardSource: co.GetCardSource());
            if (co.UseUnityCoroutines)
            {
                yield return co.GameController.StartCoroutine(coroutine);
            }
            else
            {
                co.GameController.ExhaustCoroutine(coroutine);
            }

            var irradiateEffect = new OnPhaseChangeStatusEffect(co.CardWithoutReplacements, IrradiationEffectFunction, $"{cardToIrradiate.Title} is {Irradiated} until it leaves {cardToIrradiate.Location.GetFriendlyName()}.", new TriggerType[] { TriggerType.Hidden }, cardToIrradiate);
            irradiateEffect.CardMovedExpiryCriteria.Card = cardToIrradiate;

            coroutine = co.GameController.AddStatusEffect(irradiateEffect, true, co.GetCardSource());
            if (co.UseUnityCoroutines)
            {
                yield return co.GameController.StartCoroutine(coroutine);
            }
            else
            {
                co.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public static IEnumerator ClearIrradiation(this CardController co,  Card card)
        {
            var marks = card?.NextToLocation.Cards.Where((Card c) => c.IsIrradiatedCard());
            if (marks != null && marks.Any())
            {
                IEnumerator coroutine = co.BulkMoveCard(co.DecisionMaker, marks, co.TurnTaker.OffToTheSide, false, false, co.DecisionMaker, false);
                if (co.UseUnityCoroutines)
                {
                    yield return co.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    co.GameController.ExhaustCoroutine(coroutine);
                }
            }
            var irradiationEffects = co.GameController.StatusEffectControllers.Where((StatusEffectController sec) => sec.StatusEffect is OnPhaseChangeStatusEffect opc && (opc.MethodToExecute == IrradiationEffectFunction && opc.CardMovedExpiryCriteria.Card == card)).Select(sec => sec.StatusEffect).ToList();
            foreach (StatusEffect effect in irradiationEffects)
            {
                IEnumerator coroutine = co.GameController.ExpireStatusEffect(effect, co.GetCardSource());
                if (co.UseUnityCoroutines)
                {
                    yield return co.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    co.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }

}