using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class FrenziedMeleeCardController : QuicksilverBaseCardController
    {
        public FrenziedMeleeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";
        public override void AddTriggers()
        {
            //Increase all damage dealt by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => true, 1);
            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt) && base.IsVillain(base.GameController.ActiveTurnPhase.TurnTaker) && !action.DamageSource.IsHero && IsHero(action.Target), (DealDamageAction action) => this.RedirectDamageResponse(action), TriggerType.RedirectDamage, TriggerTiming.Before, isActionOptional: true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeDamageDealt), TriggerType.Hidden);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction action)
        {
            //The first time...
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
            //...redirect that damage to {Quicksilver}.
            IEnumerator coroutine = base.RedirectDamage(action, TargetType.HighestHP, (Card c) => c == base.CharacterCard, optional: true);
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

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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