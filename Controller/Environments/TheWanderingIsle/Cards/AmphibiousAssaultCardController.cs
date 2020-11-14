using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class AmphibiousAssaultCardController : CardController
    {
        public AmphibiousAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.PlayCardResponse), new TriggerType[] { TriggerType.PlayCard }, (PhaseChangeAction pca) => this.WasHeroCardPlayedThisRound(), false);
        }

        public override IEnumerator Play()
        {
            // When this card enters play, the { H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            List<Card> heroTargetsChosen = new List<Card>();
            Card heroTarget;
            //Find the villain targets with the lowest HP
            List<Card> storedResults = new List<Card>();
            IEnumerator findVillainSource = base.GameController.FindTargetsWithLowestHitPoints(1, 2, (Card c) => c.IsVillainTarget, storedResults, null, null, false, false, null, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findVillainSource);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findVillainSource);
            }

            if (storedResults != null)
            {
                List<Card> lowestVillains = new List<Card>();
                foreach (Card c in storedResults)
                {
                    lowestVillains.Add(c);
                }

                //the two lowest villain targets each deal 3 lightning damage to a different target
                foreach (Card villainSource in lowestVillains)
                {
                    List<DealDamageAction> damageDealt = new List<DealDamageAction>();
                    IEnumerator dealDamage = base.DealDamage(villainSource, (Card c) => c.IsTarget && c.IsHero && !heroTargetsChosen.Contains(c), (Card c) => new int?(3), DamageType.Lightning, false, false, damageDealt, null, null, false, new Func<int>(this.GetNumberOfTargets), null, false, true);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(dealDamage);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(dealDamage);
                    }
                    //add the targetted heroes to the list of heroes who have already been dealt damage
                    if (damageDealt != null)
                    {
                        heroTarget = damageDealt.FirstOrDefault<DealDamageAction>().Target;
                        heroTargetsChosen.Add(heroTarget);
                    }
                }

            }

            yield break;
        }

        private IEnumerator PlayCardResponse(PhaseChangeAction pca)
        {
            //play the top card of the villain deck.
            IEnumerator play = base.PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => ttc.IsVillain && !ttc.TurnTaker.IsScion, (Location l) => l.IsVillain, base.TurnTaker, false, true, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(play);
            }
            else
            {
                base.GameController.ExhaustCoroutine(play);
            }

            //Then, destroy this card
            IEnumerator destroy = this.GameController.DestroyCard(this.DecisionMaker, this.Card, false, null, null, null, null, null, null, null, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(destroy);
            }
            else
            {
                this.GameController.ExhaustCoroutine(destroy);
            }

            yield break;
        }

        /// <summary>
        /// Gets all PlayCardEntries from the journal from this round
        /// </summary>
        /// <returns>IEnumerable with only the PlayCardEntries from this round</returns>
        private IEnumerable<PlayCardJournalEntry> PlayCardEntriesThisRound()
        {
            return from e in base.GameController.Game.Journal.PlayCardEntries()
                   where e.Round == this.Game.Round
                   select e;
        }

        /// <summary>
        /// Determine if a hero card was played this round
        /// </summary>
        /// <returns>Whether a hero card was played this round</returns>
        private bool WasHeroCardPlayedThisRound()
        {
            int numHeroCardsPlayedThisRound = (from e in this.PlayCardEntriesThisRound()
                                               where e.CardPlayed != null && e.CardPlayed.IsHero == true
                                               select e).Count<PlayCardJournalEntry>();

            return numHeroCardsPlayedThisRound > 0;
        }

        /// <summary>
        /// method wrapper for getting the number 1
        /// </summary>
        /// <returns>1</returns>
        private int GetNumberOfTargets()
        {
            return 1;
        }
    }
}
