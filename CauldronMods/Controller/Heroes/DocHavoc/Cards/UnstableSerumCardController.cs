using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class UnstableSerumCardController : CardController
    {
        //==============================================================
        // When this card enters play, 1 target regains 2HP.
        // At the end of your turn, you may destroy 1 ongoing or environment card. If you do, destroy this card.
        //==============================================================

        public static readonly string Identifier = "UnstableSerum";

        private const int HpGain = 2;

        public UnstableSerumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of your turn, you may destroy 1 ongoing or environment card. If you do, destroy this card.
            this.AddEndOfTurnTrigger(
                (TurnTaker tt) =>
                    tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(this.DestroyCardAndDestroyResponse), new TriggerType[]
                {
                    TriggerType.DestroyCard,
                    TriggerType.DestroySelf
                });

        }

        public override IEnumerator Play()
        {
            //When this card enters play, 1 target regains 2HP.
            IEnumerator gainHpRoutine = this.GameController.SelectAndGainHP(this.HeroTurnTakerController, HpGain, cardSource: this.GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return gainHpRoutine;
            }
            else
            {
                base.GameController.ExhaustCoroutine(gainHpRoutine);
            }
        }

        private IEnumerator DestroyCardAndDestroyResponse(PhaseChangeAction phaseChange)
        {
            //you may destroy 1 ongoing or environment card.
            List<DestroyCardAction> storedDestroyResults = new List<DestroyCardAction>();

            IEnumerator destroyCardRoutine = base.GameController.SelectAndDestroyCard(this.HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsEnvironment || IsOngoing(c)), true, storedDestroyResults, cardSource:
                this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }

            if (!base.DidDestroyCard(storedDestroyResults))
            {
                yield break;
            }

            //If you do, destroy this card.
            IEnumerator destroySelfRoutine = base.DestroyThisCardResponse(phaseChange);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroySelfRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroySelfRoutine);
            }
        }
    }
}
