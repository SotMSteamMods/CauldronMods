using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public class SpeedDemonGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public SpeedDemonGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"Crash cards in {HeroTurnTaker.Hand.GetFriendlyName()}: {TrueCrashInHand} out of {HeroTurnTaker.Hand.NumberOfCards}");
        }
        public override IEnumerator UsePower(int index = 0)
        {

            Func<bool> showDecisionIf = delegate
            {
                int twiceCrash = TrueCrashInHand * 2;
                int handSize = HeroTurnTaker.NumberOfCardsInHand;
                if(twiceCrash < handSize && twiceCrash + 2 >= handSize)
                {
                    //under threshold, but barely
                    return true;
                }
                if(handSize > 0 && twiceCrash >= handSize && twiceCrash - 2 < handSize)
                {
                    //over threshold, but barely
                    return true;
                }
                return false;
            };
            var storedModifier = new List<int>();
            IEnumerator coroutine = EvaluateCrashInHand(storedModifier, showDecisionIf);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int crashMod = storedModifier.FirstOrDefault();

            if((TrueCrashInHand + crashMod) * 2 >= HeroTurnTaker.NumberOfCardsInHand)
            {
                //"If at least half of the cards in your hand are crash cards, draw a card. 
                coroutine = DrawCard();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //If not, play a card."
                coroutine = SelectAndPlayCardFromHand(DecisionMaker, optional: false);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                case 1:
                    {
                        //"One player with fewer than 4 cards in their hand may play 2 cards now.",
                        var viableHeroes = GameController.AllHeroControllers.Where(httc => !httc.IsIncapacitatedOrOutOfGame && httc.HeroTurnTaker.Hand.NumberOfCards < 4 && GameController.CanPlayCards(httc, GetCardSource())).Select(httc => httc.TurnTaker);
                        if (viableHeroes.Count() == 1)
                        {
                            var hero = viableHeroes.FirstOrDefault();
                            coroutine = GameController.SendMessageAction($"{hero.ToHero().Hand.GetFriendlyName()} is the only possible choice.", Priority.Medium, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            coroutine = OptionalPlayTwoCards(hero);
                        }
                        else
                        {
                            var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, viableHeroes, SelectionType.PlayCard, numberOfCards: 2, cardSource: GetCardSource());
                            coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, OptionalPlayTwoCards);
                        }
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
                case 2:
                    {
                        //"Reduce the next damage dealt to a hero target by 2."
                        var reduceEffect = new ReduceDamageStatusEffect(2);
                        reduceEffect.TargetCriteria.IsHero = true;
                        reduceEffect.TargetCriteria.IsTarget = true;
                        reduceEffect.NumberOfUses = 1;
                        reduceEffect.CardSource = Card;

                        coroutine = AddStatusEffect(reduceEffect);
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

        private IEnumerator OptionalPlayTwoCards(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            if(heroTTC == null)
            {
                yield break;
            }

            IEnumerator coroutine = GameController.SelectAndPlayCardsFromHand(heroTTC, 2, true, 2, cardSource: GetCardSource());
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
