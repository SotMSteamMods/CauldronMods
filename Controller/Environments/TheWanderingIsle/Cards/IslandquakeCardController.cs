using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class IslandquakeCardController : CardController
    {
        public IslandquakeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //make hero targets who caused Teryx to gain HP last round to become immune to the next damage
            IEnumerator makeImmune;
            foreach (Card c in this.GetHeroesWhoCausedTeryxToGainHpLastRound())
            {
                CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
                cannotDealDamageStatusEffect.TargetCriteria.IsSpecificCard = c;
                cannotDealDamageStatusEffect.NumberOfUses = new int?(1);
                cannotDealDamageStatusEffect.UntilTargetLeavesPlay(c);
                cannotDealDamageStatusEffect.IsPreventEffect = true;
                makeImmune = base.AddStatusEffect(cannotDealDamageStatusEffect, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(makeImmune);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(makeImmune);
                }
            }

            //this card deals each target other than Teryx 4 sonic damage
            IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => c.Identifier != "Teryx", 4, DamageType.Sonic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamage);
            }

            //Then, this card is destroyed.
            IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroy);
            }

            yield break;
        }


        private IEnumerable<GainHPJournalEntry> GainHPEntriesThisRound()
        {
            IEnumerable<GainHPJournalEntry> gainHPJournalEntriesThisRound = from e in base.GameController.Game.Journal.GainHPEntries()
                                                                            where e.Round == this.Game.Round
                                                                            select e;
            return gainHPJournalEntriesThisRound;
        }


        private List<Card> GetHeroesWhoCausedTeryxToGainHpLastRound()
        {
            IEnumerable<GainHPJournalEntry> teryxGainHp = from e in this.GainHPEntriesThisRound()
                                                          where e.TargetCard != null && e.TargetCard.Identifier == "Teryx" && e.SourceCard != null && e.SourceCard.IsHero && e.SourceCard.IsTarget
                                                          select e;
            List<Card> heroes = new List<Card>();
            foreach (GainHPJournalEntry ghp in teryxGainHp)
            {
                Card c = ghp.SourceCard;
                if (c.IsTarget && c.IsHero)
                {
                    heroes.Add(c);
                }
            }

            return heroes;
        }

    }
}
