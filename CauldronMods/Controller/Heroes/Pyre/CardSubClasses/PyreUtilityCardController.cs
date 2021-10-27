using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCardController : CardController
    {
        private const string IrradiationEffectFunction = "FakeIrradiationStatusEffectFunction";
        public const string CascadeKeyword = "cascade";
        public const string Irradiated = "{Rad}";

        protected PyreUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected void ShowIrradiatedCardsInHands()
        {
            foreach (HeroTurnTaker hero in Game.HeroTurnTakers)
            {
                SpecialStringMaker.ShowListOfCardsAtLocation(hero.Hand, new LinqCardCriteria((Card c) => IsIrradiated(c), Irradiated));
            }
        }
        protected void ShowIrradiatedCount(bool reverse = false)
        {
            string descriptor = reverse ? $"non-{Irradiated}" : Irradiated;
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => GameController.AllHeroes.Where(htt => !htt.IsIncapacitatedOrOutOfGame).Select(htt => htt.Hand), new LinqCardCriteria((Card c) => IsIrradiated(c) != reverse, descriptor));
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
        protected bool IsCascade(Card c)
        {
            return GameController.DoesCardContainKeyword(c, CascadeKeyword);
        }

        protected IEnumerator IrradiateCard(Card cardToIrradiate)
        {
            if(IsIrradiated(cardToIrradiate))
            {
                yield break;
            }
            var marker = TurnTakerControllerWithoutReplacements.TurnTaker.GetAllCards(realCardsOnly: false).Where((Card c) => !c.IsRealCard && c.Location.IsOffToTheSide).FirstOrDefault();
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

                var irradiateEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, IrradiationEffectFunction, $"{cardToIrradiate.Title} is {Irradiated} until it leaves {cardToIrradiate.Location.GetFriendlyName()}.", new TriggerType[] { TriggerType.Hidden }, cardToIrradiate);
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

        protected IEnumerator SelectAndIrradiateCardsInHand(HeroTurnTakerController decisionMaker, TurnTaker playerWithHand, int maxCards, int? minCards = null, List<SelectCardDecision> storedResults = null, Func<Card, bool> additionalCriteria = null)
        {
            
            if(additionalCriteria == null)
            {
                additionalCriteria = (Card c) => true;
            }
            Func<Card, bool> handCriteria = (Card c) => c != null && c.IsInHand;
            if(playerWithHand != null)
            {
                handCriteria = (Card c) => c != null && c.Location == playerWithHand.ToHero().Hand;
            }

            var fullCriteria = new LinqCardCriteria((Card c) => handCriteria(c) && !IsIrradiated(c) && additionalCriteria(c), $"non-{Irradiated}");
            if(storedResults == null)
            {
                storedResults = new List<SelectCardDecision>();
            }
            if(minCards == null)
            {
                minCards = maxCards;
            }

            IEnumerator coroutine = GameController.SelectCardsAndDoAction(decisionMaker, fullCriteria, SelectionType.CardFromHand, IrradiateCard, maxCards, false, minCards, storedResults, cardSource: GetCardSource());
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
        protected IEnumerator ClearIrradiation(Card card)
        {
            //Log.Debug($"ClearIrradiation called on {card.Title}");
            var marks = card?.NextToLocation.Cards.Where((Card c) => !c.IsRealCard && c.Identifier == "IrradiatedMarker");
            if (marks != null && marks.Any())
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
            foreach (StatusEffect effect in irradiationEffects)
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
    }
}
