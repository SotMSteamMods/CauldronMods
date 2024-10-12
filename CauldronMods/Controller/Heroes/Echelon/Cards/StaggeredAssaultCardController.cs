using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class StaggeredAssaultCardController : TacticBaseCardController
    {
        //==============================================================
        // "At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        // "Once per turn when a hero target deals an instance of 2 or more damage, {Echelon} may deal 1 target 1 melee damage."
        //==============================================================

        public static string Identifier = "StaggeredAssault";
        private readonly string damageKey = "StaggeredAssaultFollowupDamageKey";

        public StaggeredAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(damageKey);
        }

        protected override void AddTacticEffectTrigger()
        {
            //"Once per turn when a hero target deals an instance of 2 or more damage, {Echelon} may deal 1 target 1 melee damage."
            AddTrigger((DealDamageAction dd) => dd.DamageSource.IsHeroTarget && dd.Amount >= 2 && dd.DidDealDamage && !HasBeenSetToTrueThisTurn(damageKey), DealFollowupDamageResponse, TriggerType.DealDamage, TriggerTiming.After, isActionOptional: true);
        }

        private IEnumerator DealFollowupDamageResponse(DealDamageAction dd)
        {
            var storedTarget = new List<SelectTargetDecision>();
            var targets = GameController.FindTargetsInPlay((Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()));
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, targets, storedTarget, true, damageSource: CharacterCard, damageAmount: _ => 1, damageType: DamageType.Melee, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var selectedTarget = storedTarget.FirstOrDefault()?.SelectedCard;
            if(selectedTarget != null)
            {
                SetCardPropertyToTrueIfRealAction(damageKey);
                coroutine = DealDamage(CharacterCard, selectedTarget, 1, DamageType.Melee, optional: false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
