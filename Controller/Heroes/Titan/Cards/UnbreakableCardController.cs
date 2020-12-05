using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Titan
{
    public class UnbreakableCardController : CardController
    {
        public UnbreakableCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Skip any effects which would act at the end of the villain turn.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt.IsVillain, this.UpdateStatusEffectResponse, TriggerType.ModifyStatusEffect, additionalCriteria: (PhaseChangeAction action) => Game.IsVillainTeamMode || Game.IsOblivAeonMode);
            //You may not use powers.
            base.CannotUsePowers((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //You may not draw cards.
            base.CannotDrawCards((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator UpdateStatusEffectResponse(PhaseChangeAction action)
        {
            //Remove existing Status Effect
            StatusEffect statusEffect = (from se in base.GameController.StatusEffectControllers where se.StatusEffect.CardSource == this.Card select se).FirstOrDefault().StatusEffect;
            base.GameController.StatusEffectManager.RemoveStatusEffect(statusEffect);

            //Update to current Turn Index
            IEnumerator coroutine = base.AddStatusEffect(this.CreateStatusEffect());
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

        public override IEnumerator Play()
        {
            //Immediately create skip End of Turn status effect
            IEnumerator coroutine = base.AddStatusEffect(this.CreateStatusEffect());
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

        private PreventPhaseEffectStatusEffect CreateStatusEffect()
        {
            //Skip any effects which would act at the end of the villain turn.
            //Default turn index will update as needed
            int turnIndex = 0;
            if (base.Game.ActiveTurnTaker.IsVillain)
            {
                //if this is called during the villain turn make sure it is using that turn index
                turnIndex = base.Game.TurnIndex ?? default;
            }
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.End);
            preventPhaseEffectStatusEffect.UntilCardLeavesPlay(this.Card);
            preventPhaseEffectStatusEffect.TurnIndexCriteria.EqualTo = turnIndex;
            return preventPhaseEffectStatusEffect;
        }
    }
}
