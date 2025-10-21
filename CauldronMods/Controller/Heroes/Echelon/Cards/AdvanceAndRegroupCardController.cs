using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class AdvanceAndRegroupCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, destroy this card.
        // Whenever a non-hero target is destroyed, 1 hero target regains 2HP.
        //==============================================================

        public static string Identifier = "AdvanceAndRegroup";

        private const int HpToGain = 2;

        public AdvanceAndRegroupCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.CanExtend = false;
        }

        protected override void AddTacticEffectTrigger()
        {
            // Whenever a non-hero target is destroyed, 1 hero target regains 2HP.
            base.AddTrigger<DestroyCardAction>(dca => dca.WasCardDestroyed && !IsHeroTarget(dca.CardToDestroy.Card) 
                            && dca.CardToDestroy.Card.IsTarget && base.GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()),
                this.DestroyNonHeroTargetResponse,
                new[]
                {
                    TriggerType.GainHP
                }, TriggerTiming.After, null, false, true, true);
        }

        private IEnumerator DestroyNonHeroTargetResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine = GameController.SelectAndGainHP(DecisionMaker, HpToGain, false, (Card c) => IsHeroTarget(c), 1, 1, cardSource: GetCardSource());
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
    }
}
