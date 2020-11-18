using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class FrenziedMeleeCardController : CardController
    {
        public FrenziedMeleeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //Increase all damage dealt by 1.
            base.AddTrigger(base.AddIncreaseDamageTrigger((DealDamageAction action) => true, 1));
            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            base.AddTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt") && base.GameController.ActiveTurnPhase.IsVillain && !action.DamageSource.IsHero && action.Target.IsHero, (DealDamageAction action) => this.RedirectDamageResponse(action), TriggerType.RedirectDamage, TriggerTiming.Before));
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction action)
        {
            //The first time...
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt", null);
            //...redirect that damage to {Quicksilver}.
            IEnumerator coroutine = base.RedirectDamage(action, TargetType.All, (Card c) => c == base.CharacterCard);
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