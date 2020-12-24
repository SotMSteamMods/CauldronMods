using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class LocalMicrogravityCardController : CardController
    {
        private readonly string microKey = "LocalMicrogravityPreventionKey";
        public LocalMicrogravityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(microKey).Condition = () => Game.ActiveTurnTaker.IsEnvironment;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} regains 1HP.",
            IEnumerator coroutine = GameController.GainHP(this.CharacterCard, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"The first time {Impact} would be dealt damage each environment turn, prevent that damage."
            AddTrigger((DealDamageAction dd) => dd.Target == this.CharacterCard && dd.Amount > 0 && Game.ActiveTurnPhase.IsEnvironment && !HasBeenSetToTrueThisTurn(microKey), PreventDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isActionOptional: false);

            AddAfterLeavesPlayAction((GameAction _) => ResetFlagAfterLeavesPlay(microKey), TriggerType.Hidden);
        }

        private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(microKey);
            IEnumerator coroutine = GameController.CancelAction(dd, isPreventEffect: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} deals 1 target 3 sonic damage. Destroy this card."
            int numTargets = GetPowerNumeral(0, 1);
            int numDamage = GetPowerNumeral(1, 3);
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), numDamage, DamageType.Sonic, numTargets, false, numTargets, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.DestroyCard(DecisionMaker, this.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}