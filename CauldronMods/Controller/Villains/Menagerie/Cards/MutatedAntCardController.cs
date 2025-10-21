using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class MutatedAntCardController : CardController
    {
        public MutatedAntCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt), () => "A non-villain target has dealt damage this turn.", () => "A non-villain target has not dealt damage this turn.");
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";
        private Card OriginalCardThatDealtDamage = null;

        public override void AddTriggers()
        {
            //Reduce damage dealt to this card by 1.
            base.AddReduceDamageTrigger((Card c) => c == base.Card, 1);

            //The first time a non-villain target deals damage each turn, this card deals that target 1 irreducible toxic damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt) && action.DamageSource != null && action.DamageSource.Card != null && !base.IsVillainTarget(action.DamageSource.Card) && action.DamageSource.IsTarget && action.DidDealDamage && action.Amount > 0, this.StoreCardDealingDamage, TriggerType.FirstTrigger, TriggerTiming.After);
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt) && action.DamageSource != null && action.DamageSource.Card != null && !base.IsVillainTarget(action.DamageSource.Card) && action.DamageSource.IsTarget && action.DidDealDamage && action.Amount > 0, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator StoreCardDealingDamage(DealDamageAction dd)
        {
            if (OriginalCardThatDealtDamage is null)
            {
                OriginalCardThatDealtDamage = dd.DamageSource.Card;
            }
            yield return null;
            yield break;
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //The first time a non-villain target deals damage each turn,
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
            //...this card deals that target 1 irreducible toxic damage.
            IEnumerator coroutine = base.DealDamage(base.Card, OriginalCardThatDealtDamage, 1, DamageType.Toxic, true, isCounterDamage: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            OriginalCardThatDealtDamage = null;
            yield break;
        }
    }
}
