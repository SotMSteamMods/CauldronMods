using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class JauntingReflexCardController : CardController
    {
        private readonly static string TrackingKey = "UsedJauntingReflex";

        public JauntingReflexCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} has been used this turn.");
            ss.Condition = () => Card.IsInPlayAndHasGameText && !WasNotUsedThisTurn();
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(dda => !dda.DamageSource.IsHero && IsHero(dda.Target) && dda.DidDealDamage && WasNotUsedThisTurn(), HeroDamagedResponse, TriggerType.UsePower, TriggerTiming.After, isActionOptional: true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(TrackingKey), TriggerType.Hidden);
        }

        private bool WasNotUsedThisTurn()
        {
            return !base.HasBeenSetToTrueThisTurn(TrackingKey);
        }

        private IEnumerator HeroDamagedResponse(DealDamageAction action)
        {
            List<DiscardCardAction> results = new List<DiscardCardAction>();
            var coroutine = GameController.SelectAndDiscardCard(DecisionMaker, optional: true, storedResults: results, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(results, 1))
            {
                base.SetCardPropertyToTrueIfRealAction(TrackingKey);

                coroutine = GameController.SelectAndUsePower(DecisionMaker, optional: true, cardSource: GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            int draws = GetPowerNumeral(0, 2);

            //Calling Destroy Card, then Draw Card(2) didn't work.  Only a single card would be drawn for I'm sure great reasons.
            //instead I check that Card is destroyable first.
            if (GameController.IsCardIndestructible(Card))
            {
                var coroutine = GameController.SendMessageAction($"{Card.Title} is Indestructible so no card will be drawn.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                var coroutine = DrawCards(DecisionMaker, draws);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
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
    }
}