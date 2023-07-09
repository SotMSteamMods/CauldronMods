using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MejiGuardCardController : OriphelUtilityCardController
    {
        public MejiGuardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCards(true);
        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to Guardians by 1.",
            AddReduceDamageTrigger((Card c) => IsGuardian(c), 1);
            //"At the end of the villain turn, this card deals the hero with the most cards in hand 2 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DamageResponse(PhaseChangeAction pca)
        {
            var biggestHandHero = new List<TurnTaker> { };
            IEnumerator coroutine = FindHeroWithMostCardsInHand(biggestHandHero);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (biggestHandHero.FirstOrDefault() != null)
            {
                var target = biggestHandHero.FirstOrDefault();
                coroutine = GameController.SelectTargetsAndDealDamage(FindHeroTurnTakerController(target.ToHero()),
                                                                    new DamageSource(GameController, this.Card),
                                                                    2,
                                                                    DamageType.Melee,
                                                                    1,
                                                                    false,
                                                                    1,
                                                                    additionalCriteria: (Card c) => c.Owner == target &&  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame,
                                                                    cardSource: GetCardSource());
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