using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class AroundTheCornerCardController : PhaseVillainCardController
    {
        public AroundTheCornerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(FirstTimeDestroyed),
                () => "An obstacle has been destroyed this turn.",
                () => "An obstacle has not been destroyed this turn.");
            ss.Condition = () => Card.IsInPlay;
        }

        private const string FirstTimeDestroyed = "FirstTimeDestroyed";

        public override void AddTriggers()
        {
            //The first time an Obstacle is destroyed each turn, {Phase} deals each hero target 2 radiant damage.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeDestroyed) && base.IsObstacle(action.CardToDestroy.Card) && action.WasCardDestroyed, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

            base.AddAfterLeavesPlayAction(() => ResetFlagAfterLeavesPlay(FirstTimeDestroyed));
        }

        private IEnumerator DealDamageResponse(DestroyCardAction action)
        {
            //The first time an Obstacle is destroyed each turn, {Phase} deals each hero target 2 radiant damage.
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDestroyed);
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHeroTarget(c), 2, DamageType.Radiant);
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