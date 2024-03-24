using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class FrequencyShiftCardController : PhaseVillainCardController
    {
        public FrequencyShiftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            Func<int> X = () => base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsObstacle(c) && c.IsInPlayAndHasGameText)).Count();
            //{Phase} regains X HP, where X is the number of Obstacle cards in play.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, null, X, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Phase} deals the hero target with the highest HP {H} irreducible radiant damage and...
            List<DealDamageAction> targetedHero = new List<DealDamageAction>();
            coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => Game.H, DamageType.Radiant, true,
                            selectTargetEvenIfCannotDealDamage: true,
                            storedResults: targetedHero);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var result = targetedHero.FirstOrDefault();
            if (result != null)
            {
                TurnTaker targetTurnTaker = result.OriginalTarget.Owner;
                //...destroys 1 ongoing...
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && c.Owner == targetTurnTaker, "ongoing"), false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and 1 equipment card belonging to that hero.
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c) && c.Owner == targetTurnTaker, "equipment"), false, cardSource: base.GetCardSource());
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