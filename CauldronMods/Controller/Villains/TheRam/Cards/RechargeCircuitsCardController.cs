using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RechargeCircuitsCardController : TheRamUtilityCardController
    {
        public RechargeCircuitsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && IsUpClose(tt)).Any(), () => "There is a hero Up Close", () => "There are no heroes Up Close.");
        }

        public override void AddTriggers()
        {
            //"body": "At the end of the villain turn, if no heroes are Up Close, {TheRam} regains 10HP and all Devices and Nodes regain 2HP.",
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, HealEverythingResponse, TriggerType.GainHP);
        }

        public IEnumerator HealEverythingResponse(PhaseChangeAction pc)
        {
            if (GameController.FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && IsUpClose(tt)).Any())
            {
                yield break;
            }

            IEnumerator coroutine = GameController.SendMessageAction("With no heroes Up Close, the Recharge Circuits activate!", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (RamIfInPlay != null)
            {
                coroutine = GameController.GainHP(GetRam, 10, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator message = MessageNoRamToAct(GetCardSource(), "regain HP");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
            }

            coroutine = GameController.GainHP(DecisionMaker, (Card c) => IsDeviceOrNode(c), 2, cardSource: GetCardSource());
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