using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class KataMichiCardController : TheInfernalChoirUtilityCardController
    {
        public KataMichiCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddIncreaseDamageTrigger(dda => IsVagrantHeartHiddenHeartInPlay() && DoesPlayAreaContainHiddenHeart(dda.DamageSource.Owner) && dda.DamageSource.IsHeroCharacterCard, 1);

            AddTrigger<UsePowerAction>(upa => upa.Power.Description != KataMichiPowerDesc && IsHeroCharacterCard(upa.Power.CardSource.Card), upa => UsePowerResponse(upa), new[] { TriggerType.CancelAction, TriggerType.UsePower }, TriggerTiming.Before);
        }

        private readonly string KataMichiPowerDesc = "Each hero target and each Ghost deals itself 2 cold damage.";

        private IEnumerator UsePowerResponse(UsePowerAction upa)
        {
            /*
             * "Replace all powers on hero character cards with the following power:",
			 * "[b]Power:[/b] Each hero target and each Ghost deals itself 2 cold damage."
            */

            var coroutine = GameController.SendMessageAction($"{Card.Title} corrupts {upa.HeroUsingPower.Name}'s power!", Priority.Medium, GetCardSource(), new[] { upa.Power.CardController.Card }, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //we want to inform the user about the replacement - Send Message
            //we need to worry about this card being destroyed during the power usage.  The power should not stop.
            //we need to worry about this card triggering itself - Check for replaced power descrition in Trigger

            coroutine = GameController.CancelAction(upa, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var power = GetKataMichiPower(upa.HeroUsingPower, upa.Power.CardController, upa.Power.Index, upa.Power.CardSource);

            coroutine = GameController.UsePower(power, heroUsingPower: upa.HeroUsingPower, cardSource: upa.Power.CardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private Power GetKataMichiPower(HeroTurnTakerController httc, CardController cc, int index, CardSource cardSource)
        {
            return new Power(httc, cc, KataMichiPowerDesc, KataMichiUsePower(), index, null, cardSource);
        }

        public IEnumerator KataMichiUsePower(int index = 0)
        {
            var usePowerAction = GameController.UnresolvedActions.OfType<UsePowerAction>().Last();
            var cs = usePowerAction.CardSource;
            var httc = cs.HeroTurnTakerController;
            var source = cs.Card;

            int damage = usePowerAction.Power.CardController.GetPowerNumeral(0, 2);


            var coroutine = GameController.SelectTargetsToDealDamageToSelf(httc, damage, DamageType.Cold, null, false, null,
                                allowAutoDecide: true,
                                additionalCriteria: c => IsHeroTarget(c) || IsGhost(c),
                                //stopDealingDamage: () => !source.IsInPlay || source.IsIncapacitatedOrOutOfGame || httc.IsIncapacitatedOrOutOfGame,
                                cardSource: cs);
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
