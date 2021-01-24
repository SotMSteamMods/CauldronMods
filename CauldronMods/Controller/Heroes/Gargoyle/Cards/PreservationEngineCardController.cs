using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

//DECKLIST EDIT: HP gain is first time each turn.
namespace Cauldron.Gargoyle
{
    /*
     * When {Gargoyle} destroys a target by reducing its HP below 0, increase the next damage dealt by {Gargoyle} by X, 
     * where X is the amount of negative HP that target had. The first time this happens each turn, {Gargoyle} regains 1HP.
     * Powers
     * Discard 2 cards. Draw 2 cards
     */
    public class PreservationEngineCardController : GargoyleUtilityCardController
    {
        private int TotalCardsToDiscard => GetPowerNumeral(0, 2);
        private int TotalCardsToDraw => GetPowerNumeral(1, 2);
        private const string HPGainKey = "PreservationEngineRegainHPKey";

        public PreservationEngineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(HPGainKey);
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(DestroyCardActionCriteria, DestroyCardActionResponse, TriggerType.DestroyCard, TriggerTiming.After);
            base.AddTriggers();

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(HPGainKey), TriggerType.Hidden);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = base.SelectAndDiscardCards(DecisionMaker, TotalCardsToDiscard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.DrawCards(DecisionMaker, TotalCardsToDraw);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool DestroyCardActionCriteria(DestroyCardAction destroyCardAction)
        {
            return destroyCardAction.CardSource != null &&
                destroyCardAction.CardSource.Card == base.CharacterCard &&
                destroyCardAction.CardToDestroy != null &&
                destroyCardAction.CardToDestroy.Card.IsTarget &&
                destroyCardAction.WasCardDestroyed &&
                destroyCardAction.DealDamageAction != null &&
                destroyCardAction.DealDamageAction.DidDealDamage == true &&
                destroyCardAction.DealDamageAction.TargetHitPointsAfterBeingDealtDamage < 0;
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            IEnumerator coroutine;
            int valueOfX = 0;

            if (destroyCardAction.DealDamageAction.TargetHitPointsAfterBeingDealtDamage < 0)
            {
                // When {Gargoyle} destroys a target by reducing its HP below 0, increase the next damage dealt by {Gargoyle} by X ..., 
                // where X is the amount of negative HP that target had.
                valueOfX = destroyCardAction.DealDamageAction.TargetHitPointsAfterBeingDealtDamage.Value * -1;
                coroutine = IncreaseGargoyleNextDamage(valueOfX);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // The first time this happens each turn, Gargoyle regains 1HP.
                if (!HasBeenSetToTrueThisTurn(HPGainKey))
                {
                    SetCardPropertyToTrueIfRealAction(HPGainKey);
                    coroutine = base.GameController.GainHP(this.CharacterCard, 1, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            yield break;
        }
    }
}
