using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class BaneOfIronCardController : TheInfernalChoirUtilityCardController
    {
        public BaneOfIronCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildHiddenHeartSpecialString()).Condition = () => IsVagrantHeartHiddenHeartInPlay() && GetPlayAreaContainingHiddenHeart() != null;
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsHero(c) && (IsEquipment(c) || IsOngoing(c)), "hero ongoing or equipment"));
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddReduceDamageTrigger((DealDamageAction dda) => ReductionCriteria(dda), 1, null);
            AddAfterDestroyedAction(ga => DestroyOngoingsOrEquipment(ga));
        }

        private bool ReductionCriteria(DealDamageAction dda)
        {
            if (dda.DamageSource is null || dda.DamageSource.Card is null  || !dda.DamageSource.Card.IsCharacter || !IsHero(dda.DamageSource.Card))
                return false;
            //is a hero character card
            return !DoesPlayAreaContainHiddenHeart(dda.DamageSource.Card.Owner);
        }

        private IEnumerator DestroyOngoingsOrEquipment(GameAction action)
        {
            var coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => IsHero(c) && (IsEquipment(c) || IsOngoing(c)), "hero ongoing or equipment"), H,
                                requiredDecisions: H,
                                allowAutoDecide: true,
                                cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
