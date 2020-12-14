using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class MinistryOfStrategicScienceMalichaeCharacterCardController : HeroCharacterCardController
    {
        public MinistryOfStrategicScienceMalichaeCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            var coroutine = SearchForCards(DecisionMaker, true, false, 1, 1,
                new LinqCardCriteria(c => c.DoKeywordsContain(MalichaeCardController.DjinnKeyword), "djinn"),
                false, true, false,
                shuffleAfterwards: true);
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player may use a power now.",
			 * "2 targets regain 1 HP each.",
			 * "Until the start of your next turn, whenever a hero would be dealt exactly 1 damage, prevent that damage."
             */

            switch (index)
            {
                case 0:
                    {
                        IEnumerator drawCardRoutine = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(drawCardRoutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(drawCardRoutine);
                        }
                        break;
                    }
                case 1:
                    {
                        var coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 1, false, numberOfTargets: 2, requiredDecisions: 2, allowAutoDecide: true, cardSource: GetCardSource());
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
                case 2:
                    {
                        var effect = new ImmuneToDamageStatusEffect();
                        effect.DamageAmountCriteria.EqualTo = 1;
                        effect.TargetCriteria.IsHero = true;
                        effect.UntilStartOfNextTurn(TurnTaker);

                        var coroutine = AddStatusEffect(effect, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
