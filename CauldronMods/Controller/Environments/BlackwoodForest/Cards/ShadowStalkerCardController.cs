using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class ShadowStalkerCardController : CardController
    {
        //==============================================================
        // At the end of the environment turn, destroy 1 ongoing card.
        // At the start of the environment turn this card deals each
        // hero target {H - 2} melee damage.
        // Whenever an environment card is destroyed, this card deals
        // the target with the highest HP 5 psychic damage
        //==============================================================

        public static readonly string Identifier = "ShadowStalker";

        private const int PsychicDamageToDeal = 5;

        public ShadowStalkerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHighestHP();
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, destroy 1 ongoing card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnDestroyOngoingResponse, TriggerType.DestroyCard);

            // At the start of the environment turn this card deals each hero target {H - 2} melee damage.
            AddDealDamageAtStartOfTurnTrigger(TurnTaker, Card, c => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                TargetType.All, H - 2, DamageType.Melee);

            // Whenever an environment card is destroyed, deal the target with the highest HP 5 psychic damage
            AddTrigger<DestroyCardAction>(EnvironmentCardDestroyedCriteria, DestroyEnvironmentResponse, TriggerType.DestroyCard, TriggerTiming.After);

            base.AddTriggers();
        }

        private bool EnvironmentCardDestroyedCriteria(DestroyCardAction dca)
        {
            return dca.WasCardDestroyed && dca.CardToDestroy.Card.IsEnvironment && GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource());
        }

        private IEnumerator EndOfTurnDestroyOngoingResponse(PhaseChangeAction pca)
        {
            // Destroy 1 ongoing card
            IEnumerator destroyCardRoutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker,
                new LinqCardCriteria(card => IsOngoing(card), "ongoing"),
                false, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }
        }

        private IEnumerator DestroyEnvironmentResponse(DestroyCardAction dca)
        {
            List<Card> highestHpTarget = new List<Card>();
            var damageInfo = new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), null, PsychicDamageToDeal, DamageType.Psychic);
            var coroutine = GameController.FindTargetWithHighestHitPoints(1, card => card.IsTarget, highestHpTarget,
                                dealDamageInfo: new[] { damageInfo },
                                evenIfCannotDealDamage: true,
                                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (highestHpTarget.Any())
            {
                var target = highestHpTarget.First();

                // Deal 5 psychic damage
                coroutine = this.DealDamage(this.Card, target, PsychicDamageToDeal, DamageType.Psychic, cardSource: this.GetCardSource());
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