using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class SphereOfDevastationCardController : GyrosaurUtilityCardController
    {
        public SphereOfDevastationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Discard all Crash cards in your hand. 
            var discardStorage = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.DiscardCards(DecisionMaker, new LinqCardCriteria((Card c) => c.Location == HeroTurnTaker.Hand && IsCrash(c), "crash"), discardStorage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Gyrosaur} deals 1 target X+4 melee damage, where X is 4 times the number of cards discarded this way.",
            var damageStorage = new List<DealDamageAction>();
            int damageAmount = (GetNumberOfCardsDiscarded(discardStorage) + 1) * 4;
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), damageAmount, DamageType.Melee, 1, false, 1, storedResultsDamage: damageStorage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"If {Gyrosaur} dealt more than 10 damage this way...  "
            var damage = damageStorage.FirstOrDefault();
            if(damage != null && damage.DidDealDamage && damage.Amount > 10)
            {
                //...destroy all environment cards...
                coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and each other player discards a card.
                coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                                                        new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && tt != TurnTaker && !tt.IsIncapacitatedOrOutOfGame),
                                                        SelectionType.DiscardCard,
                                                        (TurnTaker tt) => GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero()), responsibleTurnTaker: TurnTaker, cardSource: GetCardSource()), 
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
