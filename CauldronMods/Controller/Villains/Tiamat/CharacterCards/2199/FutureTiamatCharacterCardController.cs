using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class FutureTiamatCharacterCardController : TiamatSubCharacterCardController
    {
        public FutureTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
            base.SpecialStringMaker.ShowDamageDealt(new LinqCardCriteria((Card c) => c == base.Card, base.Card.Title, useCardsSuffix: false), thisTurn: true).Condition = () => Game.ActiveTurnTaker == base.TurnTaker && !base.Card.IsFlipped;
        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            (TurnTakerController as TiamatTurnTakerController).MoveStartingCards();            
        }

        private IEnumerator Discard2Spells(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, false, new LinqCardCriteria((Card c) => this.IsSpell(c), "spell"), 2,
                                    revealedCardDisplay: RevealedCardDisplay.Message,
                                    moveMatchingCardsToTrash: true);
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

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                if(Game.IsAdvanced)
                {
                    //Advanced: At the start of the game, reveal cards from the top of the villain deck until 2 spells are revealed. Discard those spells. Shuffle the other revealed cards back into the villain deck.
                    AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => base.Game.IsAdvanced && tt == base.TurnTaker, this.Discard2Spells, TriggerType.DiscardCard));
                    //we can let this be a regular start-of-turn trigger because Tiamat will immediately flip
                }
                //At the start of the villain turn, flip {Tiamat}'s villain character cards.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.FlipThisCharacterCardResponse, TriggerType.FlipCard));
            }
            else
            {
                //{Tiamat} counts as The Jaws of Winter, The Mouth of the Inferno, and The Eye of the Storm. Each spell card in the villain trash counts as Element of Ice, Element of Fire, and Element of Lightining.
                /**Addressed on all cards**/

                //At the end of the villain turn, {Tiamat} deals the hero target with the highest HP {H} energy damage. Then, if {Tiamat} deals no damage this turn, each hero target deals itself 3 projectile damage. Then flip all ruined scales and restore them to their max HP.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.FlipCard }));

                if(this.IsGameChallenge)
                {
                    //"Whenever a non-villain target deals damage to a Dragonscale, {Tiamat} deals that target {H - 1} projectile damage.",
                    base.AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.Target.IsVillainCharacterCard && dd.Target != this.Card && dd.DamageSource.Card != null && dd.DamageSource.IsTarget && !IsVillainTarget(dd.DamageSource.Card), ChallengeCounterDamageResponse, TriggerType.DealDamage, TriggerTiming.After));
                }
            }
            //When {Tiamat} is destroyed, the heroes win.
            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction action)
        {
            //...{Tiamat} deals the hero target with the highest HP {H} energy damage. 
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => Game.H, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...Then, if {Tiamat} deals no damage this turn, each hero target deals itself 3 projectile damage.
            if (!this.DidDealDamageThisTurn())
            {
                coroutine = base.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => IsHero(c), (Card c) => 3, DamageType.Projectile, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //...Then flip all ruined scales and restore them to their max HP.
            IEnumerable<Card> ruinedScales = base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsRuinedScale(c)));
            foreach (Card scale in ruinedScales)
            {
                coroutine = base.GameController.FlipCard(base.FindCardController(scale));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Dragonscales have X HP, where X = {H - 1}.
                int hp = Game.H - 1;
                if (base.Game.IsAdvanced)
                {
                    //Advanced: X = {H + 1} instead.
                    hp = Game.H + 1;
                }
                coroutine = base.GameController.MakeTargettable(card: scale, scale.MaximumHitPoints ?? hp, scale.MaximumHitPoints ?? hp, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        private IEnumerator ChallengeCounterDamageResponse(DealDamageAction dd)
        {
            //"Whenever a non-villain target deals damage to a Dragonscale, {Tiamat} deals that target {H - 1} projectile damage.",
            return DealDamage(this.Card, dd.DamageSource.Card, H - 1, DamageType.Projectile, isCounterDamage: true, cardSource: GetCardSource());
        }
        private bool IsRuinedScale(Card c)
        {
            return c.DoKeywordsContain("ruined scale");
        }
    }
}
