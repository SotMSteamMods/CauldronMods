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
            ShowCrashInHandCount(true);
        }

        public override IEnumerator Play()
        {
            int valueOfX = 0;

            //"Discard all Crash cards in your hand. 
            var discardStorage = new List<DiscardCardAction>();
            IEnumerator coroutine;
            // If user setting to auto discard is set to true, automatically choose order of crash card discarding
            if (this.ShouldAutoDiscardHand())
            {
                // While there is a crash card in hand, discard the first crash card found
                while (HeroTurnTaker.Hand.Cards.Where((Card c) => IsCrash(c)).Count() > 0)
                {
                    coroutine = GameController.DiscardCard(
                        DecisionMaker,
                        HeroTurnTaker.Hand.Cards.First((Card c) => IsCrash(c)),
                        null,
                        HeroTurnTaker,
                        discardStorage,
                        GetCardSource()
                    );
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
            else // manually choose the order to discard crash cards
            {
                coroutine = GameController.DiscardCards
                (
                    DecisionMaker,
                    new LinqCardCriteria((Card c) => c.Location == HeroTurnTaker.Hand && IsCrash(c), "crash"),
                    discardStorage,
                    cardSource: GetCardSource()
                );
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{Gyrosaur} deals 1 target X + 4 melee damage, where X is 4 times the number of cards discarded this way.",
            var damageStorage = new List<DealDamageAction>();
            valueOfX = GetNumberOfCardsDiscarded(discardStorage) * 4;
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), valueOfX + 4, DamageType.Melee, 1, false, 1, storedResultsDamage: damageStorage, cardSource: GetCardSource());
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
                                                        new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && tt != TurnTaker && !tt.IsIncapacitatedOrOutOfGame),
                                                        SelectionType.DiscardCard,
                                                        (TurnTaker tt) => GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero()), responsibleTurnTaker: TurnTaker,  cardSource: GetCardSource()), 
                                                        allowAutoDecide: true,
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

        private bool ShouldAutoDiscardHand()
        {
            if (GameController.PlayerPolicies.AutoDecideHandDiscardOrder != 0)
            {
                return GameController.PlayerPolicies.AutoDecideHandDiscardOrder == AlwaysSmartNever.Smart;
            }
            return true;
        }
    }
}
