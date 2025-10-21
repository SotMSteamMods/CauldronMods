using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class StalkerAugCardController : AugCardController
    {
        public StalkerAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP().Condition = () => base.Card.IsInPlayAndNotUnderCard;
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals each hero target except the hero with the lowest HP {H - 2} irreducible energy damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageExceptLowestResponse, TriggerType.DealDamage);
        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //At the end of the villain turn, {SwarmEater} deals each other target 1 irreducible energy damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => CanAbsorbEffectTrigger() && tt == base.TurnTaker, pca => this.AbsorbDealDamageResponse(pca, absorbingCard), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageExceptLowestResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.DealDamage(base.Card, (Card card) => IsHeroTarget(card), Game.H - 2, DamageType.Energy, true, exceptFor: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria((Card c) => IsHeroTarget(c), "the hero target with the lowest HP")));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.FindActiveHeroTurnTakerControllers().Count() == 1)
            {
                coroutine = base.GameController.SendMessageAction(base.Card.Title + " does not deal damage because there is only one hero target.", Priority.Medium, base.GetCardSource(null), null, false);
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

        private IEnumerator AbsorbDealDamageResponse(PhaseChangeAction action, Card absorbingCard)
        {
            //...{SwarmEater} deals each other target 1 irreducible energy damage.
            IEnumerator coroutine = base.DealDamage(absorbingCard, (Card c) => c != absorbingCard, (Card c) => 1, DamageType.Energy, true);
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
