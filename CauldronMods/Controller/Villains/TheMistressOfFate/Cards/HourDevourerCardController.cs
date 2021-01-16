using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class HourDevourerCardController : TheMistressOfFateUtilityCardController
    {
        public HourDevourerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => NumFaceUpDays() == 1, () => $"There is 1 face up day card.", () => $"There are {NumFaceUpDays()} face up day cards.");
        }

        private int NumFaceUpDays()
        {
            return GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsDay(c)).Count();
        }
        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals each non-villain target X sonic damage, where X is 3 times the number of Day cards face up.",
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
            //"When damage dealt by a target destroys this card, that target becomes immune to damage until the start of its next turn."
            AddWhenDestroyedTrigger(MakeDestroyerImmuneResponse, new TriggerType[] { TriggerType.CreateStatusEffect, TriggerType.MakeImmuneToDamage });
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pc)
        {
            Func<Card, int?> threeTimesFaceUpDays = (Card target) => 3 * NumFaceUpDays();

            IEnumerator coroutine = DealDamage(Card, (Card c) => !c.IsVillain, threeTimesFaceUpDays, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
        private IEnumerator MakeDestroyerImmuneResponse(DestroyCardAction dc)
        {
            if(dc.ActionSource is DealDamageAction dd && dd.DamageSource.IsTarget)
            {
                var destroyer = dd.DamageSource.Card;
                var immuneEffect = new ImmuneToDamageStatusEffect();
                immuneEffect.TargetCriteria.IsSpecificCard = destroyer;
                immuneEffect.UntilTargetLeavesPlay(destroyer);
                immuneEffect.UntilStartOfNextTurn(destroyer.Owner);

                IEnumerator coroutine = AddStatusEffect(immuneEffect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
