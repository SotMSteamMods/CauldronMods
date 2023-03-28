using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class SlingshotTrajectoryCardController : CardController
    {
        public SlingshotTrajectoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy any number of your ongoing cards. {Impact} deals X targets 2 projectile damage each, where X is 1 plus the number of ongoing cards destroyed this way."
            int numDamage = GetPowerNumeral(0, 2);
            int numTargetIncrease = GetPowerNumeral(1, 1);
            var storedDestroys = new List<DestroyCardAction> { };
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && c.Owner == this.TurnTaker, "ongoing"), null, false, 0, storedResultsAction: storedDestroys, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int numTargets = GetNumberOfCardsDestroyed(storedDestroys) + numTargetIncrease;
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), numDamage, DamageType.Projectile, numTargets, false, numTargets, cardSource: GetCardSource());
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
    }
}