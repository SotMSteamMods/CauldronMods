using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocsFlaskCardController : CardController
    {
        public static readonly string Identifier = "DocsFlask";
        private const int HpGain = 1;

        public DocsFlaskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //==============================================================
            // At the start of your turn, 1 hero target regains 1 HP
            //==============================================================

            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.StartOfTurnResponse), TriggerType.GainHP);

        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //1 hero target regains 1 HP
            IEnumerator coroutine = GameController.SelectAndGainHP(DecisionMaker, 1,
                        additionalCriteria: c => IsHeroTarget(c) && c.IsInPlayAndHasGameText,
                        numberOfTargets: 1,
                        requiredDecisions: 1,
                        cardSource: GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Each  hero target regains 1 HP.
            //==============================================================

            int gainHpAmount = this.GetPowerNumeral(0, HpGain);
            IEnumerator coroutine = this.GameController.GainHP(DecisionMaker, c => IsHeroTarget(c) && c.IsInPlayAndHasGameText, gainHpAmount, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {

                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
