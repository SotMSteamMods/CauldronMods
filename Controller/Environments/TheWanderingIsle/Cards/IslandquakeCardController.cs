using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class IslandquakeCardController : TheWanderingIsleCardController
    {
        public IslandquakeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //make hero targets who caused Teryx to gain HP last round to become immune to the next damage
            IEnumerator coroutine;
            foreach (Card c in this.GetHeroesWhoCausedTeryxToGainHpLastRound())
            {
                CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
                cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = base.Card;
                cannotDealDamageStatusEffect.TargetCriteria.IsSpecificCard = c;
                cannotDealDamageStatusEffect.UntilEndOfPhase(base.TurnTaker, Phase.Start);
                cannotDealDamageStatusEffect.IsPreventEffect = true;
                coroutine = base.AddStatusEffect(cannotDealDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //this card deals each target other than Teryx 4 sonic damage
            coroutine = base.DealDamage(base.Card, (Card c) => c.Identifier != TeryxIdentifier, 4, DamageType.Sonic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, this card is destroyed.
            coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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

        private List<Card> GetHeroesWhoCausedTeryxToGainHpLastRound()
        {
            return base.GameController.Game.Journal.GainHPEntries()
                        .Where(e => e.Round == this.Game.Round && e.TargetCard.Identifier == TeryxIdentifier && e.SourceCard.IsHero && e.SourceCard.IsTarget)
                        .Select(e => e.SourceCard)
                        .ToList();
        }

    }
}
