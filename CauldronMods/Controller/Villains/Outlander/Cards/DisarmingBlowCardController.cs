using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DisarmingBlowCardController : OutlanderUtilityCardController
    {
        public DisarmingBlowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2);
        }

        public override IEnumerator Play()
        {
            //{Outlander} deals the 2 non-villain targets with the highest HP 3 melee damage each.
            //Any hero damaged this way discards 1 card.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => !IsVillain(c) && c.IsTarget, (Card c) => 3, DamageType.Melee, numberOfTargets: () => 2, storedResults: storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResults.Any(dd => dd.Target.IsHeroCharacterCard && dd.DidDealDamage))
            {
                List<TurnTaker> heroesThatTookDamage = storedResults.Where(dd => dd.Target.IsHeroCharacterCard && dd.DidDealDamage).Select(dd => dd.Target.Owner).Distinct().ToList();
                List<SelectTurnTakerDecision> storedHero = new List<SelectTurnTakerDecision>();
                coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.DiscardCard, false, true, storedHero, heroCriteria: new LinqTurnTakerCriteria(tt => heroesThatTookDamage.Contains(tt)), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(!DidSelectTurnTaker(storedHero) && !storedHero.First().AutoDecided)
                {
                    yield break;
                }

                // default to first hero in case decision was auto-decided
                TurnTaker selectedHero = heroesThatTookDamage.First();

                if (DidSelectTurnTaker(storedHero))
                {
                    selectedHero = GetSelectedTurnTaker(storedHero);
                }

                heroesThatTookDamage.Remove(selectedHero);
                coroutine = GameController.SelectAndDiscardCard(FindHeroTurnTakerController(selectedHero.ToHero()), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (heroesThatTookDamage.Count() > 0)
                {
                    TurnTaker otherHero = heroesThatTookDamage.First();
                    coroutine = GameController.SelectAndDiscardCard(FindHeroTurnTakerController(otherHero.ToHero()), cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
                
            }
            yield break;
        }

    }
}
