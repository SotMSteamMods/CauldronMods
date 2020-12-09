using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Phase
{
    public class BlockedSightlineCardController : CardController
    {
        public BlockedSightlineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever {Phase} would be dealt damage, reduce that damage to 0.
            base.AddReduceDamageToSetAmountTrigger((DealDamageAction action) => action.Target == base.CharacterCard, 0);
            //When this card is destroyed, {Phase} deals the 2 hero targets with the highest HP {H} irreducible radiant damage each.
            base.AddWhenDestroyedTrigger(this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction action)
        {
            //...{Phase} deals the 2 hero targets with the highest HP {H} irreducible radiant damage each.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => new int?(Game.H), DamageType.Radiant, true, numberOfTargets: () => 2);
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