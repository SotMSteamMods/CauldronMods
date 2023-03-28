using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class DecayingOrbitCardController : CardController
    {
        public DecayingOrbitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals 1 target 2 infernal damage.",
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), 2, DamageType.Infernal, 1, false, 1, cardSource: GetCardSource());
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
            //"At the start of your turn, {Impact} may deal 1 target 2 projectile damage. If he does, destroy 1 of your ongoing cards."
            AddStartOfTurnTrigger(tt => tt == this.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction _)
        {
            //Impact may deal 1 target 2 projectile damage.
            List<DealDamageAction> storedDamage = new List<DealDamageAction> { };
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), 2, DamageType.Projectile, 1, false, 0, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDealDamage(storedDamage))
            {
                //If he does, destroy 1 of your ongoing cards.
                coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Owner == this.TurnTaker && IsOngoing(c), "ongoing"), false, cardSource: GetCardSource());
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
    }
}