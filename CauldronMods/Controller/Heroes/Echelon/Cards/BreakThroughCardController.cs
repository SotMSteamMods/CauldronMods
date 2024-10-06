using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class BreakThroughCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, destroy this card.
        // Once during their turn, when 1 of a player's targets would deal damage,
        // they may increase that damage by 2
        //==============================================================

        public static string Identifier = "BreakThrough";
        private readonly string damageKey = "BreakThroughDamageUsedThisTurn";

        public override bool AllowFastCoroutinesDuringPretend 
        {
            get
            {
                return HasBeenSetToTrueThisTurn(damageKey);
            }
        }
        public BreakThroughCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.CanExtend = false;
            SpecialStringMaker.ShowHasBeenUsedThisTurn(damageKey);
        }

        protected override void AddTacticEffectTrigger()
        {
            // Once during their turn, when 1 of a player's targets would deal damage,
            // they may increase that damage by 2
            AddTrigger((DealDamageAction dd) => dd.DamageSource.IsTarget && dd.DamageSource.Card.Owner.IsPlayer && dd.DamageSource.Owner.IsActiveTurnTaker && !HasBeenSetToTrueThisTurn(damageKey),
                                    MayBoostDamageResponse,
                                    TriggerType.IncreaseDamage,
                                    TriggerTiming.Before,
                                    isActionOptional: true);
        }

        private IEnumerator MayBoostDamageResponse(DealDamageAction dd)
        {
            var storedYesNo = new List<YesNoCardDecision>();
            var player = FindHeroTurnTakerController(dd.DamageSource.Owner.ToHero());
            if (player == null)
            {
                yield break;
            }

            IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.Custom, this.Card, dd, storedYesNo, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(storedYesNo))
            {
                SetCardPropertyToTrueIfRealAction(damageKey);
                coroutine = GameController.IncreaseDamage(dd, 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to increase this damage by 2?", "Should they increase this damage by 2?", "Vote for increasing this damage by 2?", "increasing this damage by 2");

        }
    }
}
