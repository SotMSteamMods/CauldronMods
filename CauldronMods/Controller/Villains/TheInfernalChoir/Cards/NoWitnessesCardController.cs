using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class NoWitnessesCardController : TheInfernalChoirUtilityCardController
    {
        public NoWitnessesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(numberOfTargets: H - 1);
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> result = new List<DealDamageAction>();
            var coroutine = DealDamageToLowestHP(CharacterCard, 1, c =>  IsHeroCharacterCard(c), c => 3, DamageType.Infernal,
                                storedResults: result,
                                numberOfTargets: H - 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var turnTakers = result.Where(dda => IsHero(dda.Target) && dda.Amount > 0 && dda.DidDealDamage && !dda.Target.IsIncapacitatedOrOutOfGame).Select(dda => dda.Target.Owner).Distinct().ToList();
            List<SelectTurnTakerDecision> decision = new List<SelectTurnTakerDecision>();
            coroutine = GameController.SelectTurnTaker(DecisionMaker, SelectionType.DrawCard, decision,
                            allowAutoDecide: true,
                            additionalCriteria: tt => turnTakers.Contains(tt),
                            numberOfCards: 1,
                            cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectTurnTaker(decision))
            {
                var drawer = GetSelectedTurnTaker(decision).ToHero();

                coroutine = DrawCard(drawer);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                foreach (var htt in turnTakers.Where(tt => tt != drawer))
                {
                    var httc = FindHeroTurnTakerController(htt.ToHero());
                    coroutine = GameController.SelectAndDiscardCard(httc, cardSource: GetCardSource());
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
}
