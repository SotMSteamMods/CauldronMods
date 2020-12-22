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
            base.AddTrigger<DestroyCardAction>(dca => dca.WasCardDestroyed && !dca.CardToDestroy.Card.IsHero 
                            && !dca.CardToDestroy.Card.IsTarget && base.GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()),
                this.DestroyNonHeroTargetResponse,
                new[]
                {
                    TriggerType.DestroyCard
                }, TriggerTiming.After, null, false, true, true);
        }

        private IEnumerator DestroyNonHeroTargetResponse(DestroyCardAction dca)
        {
            // .. 1 hero target regains 2HP.
            List<SelectTargetDecision> selectedTarget = new List<SelectTargetDecision>();
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria(c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText));
            
            IEnumerator routine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, choices, selectedTarget, selectionType: SelectionType.GainHP, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!selectedTarget.Any())
            {
                yield break;
            }

            routine = base.GameController.GainHP(selectedTarget.FirstOrDefault()?.SelectedCard, HpToGain, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}