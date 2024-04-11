using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class CellularIrradiationCardController : PyreUtilityCardController
    {
        public CellularIrradiationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.ShowIrradiatedCount(SpecialStringMaker);
        }

        public override IEnumerator Play()
        {
            //"Select a hero. 
            var storedTurnTaker = new List<SelectTurnTakerDecision>();

            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.TurnTaker, false, false, storedTurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            HeroTurnTakerController heroTTC = null;
            if(DidSelectTurnTaker(storedTurnTaker))
            {
                heroTTC = FindHeroTurnTakerController(GetSelectedTurnTaker(storedTurnTaker).ToHero());
            }

            if(heroTTC == null)
            {
                yield break;
            }

            //They activate each of the following in order if they have at least that many {PyreIrradiate} cards in their hand:

            //dynamic, in case eg. Pyre plays it on himself and uses his power to up his count
            Func<int> irradiatedCardsInHand = delegate
            {
                return heroTTC.HeroTurnTaker.Hand.Cards.Where((Card c) => c.IsIrradiated()).Count();
            };
            
            if(irradiatedCardsInHand() == 0)
            {
                coroutine = GameController.SendMessageAction($"{heroTTC.Name} has no {PyreExtensionMethods.Irradiated} cards in hand, so nothing happens.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }

            //"{1: Use a power.",
            if(irradiatedCardsInHand() >= 1)
            {
                coroutine = GameController.SelectAndUsePower(heroTTC, false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"{2: Draw a card.",
            if(irradiatedCardsInHand() >= 2)
            {
                coroutine = GameController.DrawCard(heroTTC.HeroTurnTaker);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"{3: Deal themselves 2 energy damage.",
            if(irradiatedCardsInHand() >= 3)
            {
                coroutine = GameController.SelectTargetsToDealDamageToSelf(heroTTC, 2, DamageType.Energy, 1, false, 1, additionalCriteria: (Card c) =>  IsHeroCharacterCard(c) && c.Owner == heroTTC.TurnTaker, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"{4: Play a card."
            if(irradiatedCardsInHand() >= 4)
            {
                coroutine = GameController.SelectAndPlayCardFromHand(heroTTC, false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
