using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class LeechFieldCardController : GargoyleUtilityCardController
    {
        // "Once per turn, when {Gargoyle} deals or is dealt damage, or when a non-hero target is dealt damage, 
        // you may reduce that damage by 1 and increase the next damage dealt by {Gargoyle} by 1."
        private const string FirstTimeWouldBeDealtDamage = "OncePerTurn";
        
        private struct PreventInfo
        {
            public bool preventDamage;
            public Guid ddaGuid;
        };

        private PreventInfo? selectedReaction;

        public LeechFieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeWouldBeDealtDamage);
        }

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                // This allows us to make decisions during damage actions.
                return false;
            }
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(DealDamageCritera, DealDamageResponse, new TriggerType[] { TriggerType.ModifyDamageAmount }, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private bool DealDamageCritera(DealDamageAction dealDamageAction)
        {
            // "Once per turn, when {Gargoyle} deals or is dealt damage, or when a non-hero target is dealt damage, 
            return !HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage) &&
                dealDamageAction.Amount > 0 && 
                (
                    dealDamageAction.DamageSource?.Card == CharacterCard ||
                    dealDamageAction.Target == CharacterCard ||
                    ! IsHeroTarget(dealDamageAction.Target)
                );
        }

        private IEnumerator DealDamageResponse(DealDamageAction dealDamageAction)
        {
            if (!selectedReaction.HasValue || selectedReaction.Value.ddaGuid != dealDamageAction.InstanceIdentifier)
            {
                var storedResults = new List<YesNoCardDecision>();
                var e = GameController.MakeYesNoCardDecision(
                    DecisionMaker,
                    SelectionType.ReduceDamageTaken,
                    card: Card,
                    action: dealDamageAction,
                    storedResults: storedResults,
                    cardSource: GetCardSource()
                );

                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                selectedReaction = new PreventInfo { preventDamage = DidPlayerAnswerYes(storedResults), ddaGuid = dealDamageAction.InstanceIdentifier };
            }

            if (selectedReaction.HasValue && selectedReaction.Value.preventDamage)
            {
                SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage, gameAction: dealDamageAction);

                var e = GameController.ReduceDamage(dealDamageAction, 1, null, GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                if (IsRealAction(dealDamageAction))
                {
                    e = IncreaseGargoyleNextDamage(1, dealDamageAction.DamageSource.Card == CharacterCard ? dealDamageAction : null);
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }
                }
            }

            if (IsRealAction(dealDamageAction))
            {
                selectedReaction = null;
            }
        }
    }
}
