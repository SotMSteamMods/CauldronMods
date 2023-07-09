using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class LasherAugCardController : AugCardController
    {
        public LasherAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsInPlayAndNotUnderCard;
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage and destroys {H - 2} hero ongoing and/or equipment cards.
            base.AddEndOfTurnTrigger(tt => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageAndDestroyResponse, new[] { TriggerType.DealDamage, TriggerType.DestroyCard });
        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //Absorb: at the start of the villain turn, destroy 1 hero ongoing or equipment card.
            base.AddStartOfTurnTrigger(tt => CanAbsorbEffectTrigger() && tt == base.TurnTaker, pca => this.AbsorbDestroyResponse(pca, absorbingCard), TriggerType.DestroyCard);
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction action)
        {
            //this card deals the hero target with the highest HP {H} projectile damage...
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, c => IsHeroTarget(c), c => Game.H, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...and destroys {H - 2} hero ongoing and/or equipment cards.
            coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria(c => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), Game.H - 2, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AbsorbDestroyResponse(PhaseChangeAction action, Card absorbingCard)
        {
            //...destroy 1 hero ongoing or equipment card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria(c => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), 1, responsibleCard: absorbingCard, cardSource: base.GetCardSource());
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
}