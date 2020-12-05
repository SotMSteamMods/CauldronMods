using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class UnbreakableCardController : CardController
    {
        public UnbreakableCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Skip any effects which would act at the end of the villain turn.
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.End);
            preventPhaseEffectStatusEffect.UntilCardLeavesPlay(this.Card);
            preventPhaseEffectStatusEffect.CardCriteria.IsVillain = true;
            IEnumerator coroutine = base.AddStatusEffect(preventPhaseEffectStatusEffect);
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

        public override void AddTriggers()
        {
            //You may not use powers.
            base.CannotUsePowers((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //You may not draw cards.
            base.CannotDrawCards((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}