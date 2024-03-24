using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Gray
{
    public class IrradiatedTouchCardController : CardController
    {
        public IrradiatedTouchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP(2);
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, {Gray} deals the hero target with the second highest HP {H - 2} melee and {H - 2} energy damage.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, new TriggerType[] { TriggerType.DealDamage });
            //When this card is destroyed, {Gray} deals the hero target with the highest HP 2 energy damage.
            base.AddWhenDestroyedTrigger((DestroyCardAction action) => base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => new int?(2), DamageType.Energy), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //{Gray} deals the hero target with the second highest HP {H - 2} melee and {H - 2} energy damage.
            IEnumerator coroutine = base.DealMultipleInstancesOfDamageToHighestLowestHP(new List<DealDamageAction>
            {
                new DealDamageAction(base.GetCardSource(),new DamageSource(base.GameController,base.CharacterCard),null,Game.H -2,DamageType.Melee),
                new DealDamageAction(base.GetCardSource(),new DamageSource(base.GameController,base.CharacterCard),null,Game.H -2,DamageType.Energy)
            }, (Card c) => IsHeroTarget(c), HighestLowestHP.HighestHP, 2);
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