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

        public static string Identifier = "UnstableSerum";

        private const int HpGain = 2;

        public UnstableSerumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger(
                (TurnTaker tt) =>
                    tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(this.DestroyCardAndDestroyResponse), new TriggerType[]
                {
                    TriggerType.DestroyCard,
                    TriggerType.DestroySelf
                }, null, false);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator gainHpRoutine = this.GameController.SelectAndGainHP(this.HeroTurnTakerController, HpGain, 
                false, null, 1, cardSource: this.GetCardSource());

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
            List<DestroyCardAction> storedDestroyResults = new List<DestroyCardAction>();

            IEnumerator destroyCardRoutine = base.GameController.SelectAndDestroyCard(this.HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsEnvironment || c.IsOngoing), true, storedDestroyResults, null, 
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

            IEnumerator destroySelfRoutine = base.GameController.DestroyCard(this.DecisionMaker, this.Card,
                false, null, null, null, null, null,
                null, null, null, base.GetCardSource(null));

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
