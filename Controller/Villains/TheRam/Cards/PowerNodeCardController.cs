using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class PowerNodeCardController : TheRamUtilityCardController
    {
        public PowerNodeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers;
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are not Up Close.",
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource.IsHero && dd.DamageSource.IsCard && !IsUpClose(dd.DamageSource.Card));
            //"At the end of the villain turn, play the top card of the villain deck, and all Devices and Nodes regain 1HP."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayCardAndHealResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.GainHP });
        }

        private IEnumerator PlayCardAndHealResponse(PhaseChangeAction pc)
        {
            IEnumerator coroutine = PlayTheTopCardOfTheVillainDeckResponse(pc);
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.GainHP(DecisionMaker,
                                            (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && IsDeviceOrNode(c),
                                            1,
                                            cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}