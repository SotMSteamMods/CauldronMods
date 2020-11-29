using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class LasherAugCardController : AugCardController
    {
        public LasherAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            }
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage and destroys {H - 2} hero ongoing and/or equipment cards.
            base.AddEndOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageAndDestroyResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });

            //Absorb: at the start of the villain turn, destroy 1 hero ongoing or equipment card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => base.Card.Location.IsUnderCard && tt == base.TurnTaker, this.AbsorbDestroyResponse, TriggerType.DestroyCard);
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction action)
        {
            //this card deals the hero target with the highest HP {H} projectile damage...
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => Game.H, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...and destroys {H - 2} hero ongoing and/or equipment cards.
            coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), Game.H - 2, cardSource: base.GetCardSource());
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

        private IEnumerator AbsorbDestroyResponse(PhaseChangeAction action)
        {
            //...destroy 1 hero ongoing or equipment card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), 1, cardSource: base.GetCardSource());
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