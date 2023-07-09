using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.TheMistressOfFate
{
    public class MemoryOfTomorrowCardController : TheMistressOfFateUtilityCardController
    {
        public MemoryOfTomorrowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"When this card enters play, move it next to the right-most face up Day card.
            var lastDay = DayCardsInOrder().Where((Card c) => c.IsInPlayAndHasGameText).LastOrDefault();
            if(lastDay != null)
            {
                storedResults.Add(new MoveCardDestination(lastDay.NextToLocation));
                return DoNothing();
            }
            else
            {
                return base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            }
        }
        public override IEnumerator Play()
        {
            //"When this card enters play... {TheMistressOfFate} deals each hero 5 sonic damage and 5 cold damage.",
            var damages = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 5, DamageType.Sonic),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 5, DamageType.Cold)
            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damages, (Card c) =>  IsHeroCharacterCard(c));
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

        public override void AddTriggers()
        {
            //"Each time the Day card next to this one is flipped face up, 1 hero may draw a card and play a card."
            AddTrigger((FlipCardAction fca) => fca.CardToFlip.Card == GetCardThisCardIsNextTo() && !fca.ToFaceDown, DrawAndPlayResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard }, TriggerTiming.After);
        }

        private IEnumerator DrawAndPlayResponse(GameAction _)
        {
            var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, GameController.AllTurnTakers.Where((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame), SelectionType.PlayCard, isOptional: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DrawACardAndPlayACard);
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

        private IEnumerator DrawACardAndPlayACard(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            if(heroTTC != null)
            {
                IEnumerator coroutine = GameController.DrawCard(heroTTC.HeroTurnTaker, optional: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.SelectAndPlayCardFromHand(heroTTC, true, cardSource: GetCardSource());
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
