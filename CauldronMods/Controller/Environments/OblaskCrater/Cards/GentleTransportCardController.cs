using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.OblaskCrater
{
    public class GentleTransportCardController : OblaskCraterUtilityCardController
    {
        /* 
         * At the end of the environment turn, move this card next to a hero. Then the predator card with the highest HP deals
         * each target in this card's play area {H - 1} melee damage. 
         * If the hero next to this card plays a card with a power on it, they may immediately use a power on that card.
         */
        public GentleTransportCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria(c => IsPredator(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "predator")).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, PhaseChangeActionResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DealDamage });
            base.AddTrigger<CardEntersPlayAction>((cpe) => cpe.CardEnteringPlay.HasPowers && base.GetCardThisCardIsNextTo() == cpe.CardEnteringPlay.Owner.CharacterCard, PlayCardActionResponse, TriggerType.UsePower, TriggerTiming.After);
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: false);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            List<SelectCardDecision> storedSelectHeroResults = new List<SelectCardDecision>();
            List<SelectCardDecision> storedSelectPredatorResults = new List<SelectCardDecision>();
            IEnumerable<Card> highestPredators;
            IEnumerable<Card> targetsInPlayArea;
            IEnumerator coroutine;
            Card selectedHero;

            coroutine = base.GameController.SelectHeroCharacterCard(base.DecisionMaker, SelectionType.MoveCardNextToCard, storedSelectHeroResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedSelectHeroResults))
            { 
                selectedHero = GetSelectedCard(storedSelectHeroResults);
                bool showMessage = FindCardsWhere(c => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Count() == 1;
                // move this card next to a hero
                coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, selectedHero.NextToLocation,
                            showMessage: showMessage,
                            flipFaceDown: false,
                            cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // Then the predator card with the highest HP deals each target in this card's play area {H - 1} melee damage. 
                highestPredators = base.GameController.FindAllTargetsWithHighestHitPoints(1, (card) => IsPredator(card), base.GetCardSource());
                coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.HighestHP, new LinqCardCriteria((lcc) => highestPredators.Contains(lcc), "predator cards in play", useCardsSuffix: false), storedSelectPredatorResults, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                
                if (storedSelectPredatorResults != null && storedSelectPredatorResults.Count() > 0)
                {
                    targetsInPlayArea = selectedHero.Owner.GetPlayAreaCards().Where(card => card.IsTarget);
                    coroutine = base.DealDamage(storedSelectPredatorResults.FirstOrDefault().SelectedCard, card => targetsInPlayArea.Contains(card), base.H - 1, DamageType.Melee);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }

        private IEnumerator PlayCardActionResponse(CardEntersPlayAction cardEnteringPlayAction)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.SendMessageAction($"{cardEnteringPlayAction.CardEnteringPlay.Title} has a power on it! {GetCardThisCardIsNextTo().AlternateTitleOrTitle} may use that power immediately!", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.SelectAndUsePower(cardEnteringPlayAction.TurnTakerController.ToHero(), true, (power) => power.CardController.Card ==  cardEnteringPlayAction.CardEnteringPlay, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }
    }
}