using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CypherCharacterCardController : CypherBaseCharacterCardController
    {

        private const int PowerCardsToDraw = 1;
        private const int Incapacitate1CardsToDraw = 3;
        private const int Incapacitate2CardsToPlay = 3;
        private const int Incapacitate3HpGain = 1;

        public CypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringAugmentedHeroes();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // One augmented hero may draw a card now.

            int drawNumeral = base.GetPowerNumeral(0, PowerCardsToDraw);

            IEnumerator routine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController,
                additionalCriteria: new LinqTurnTakerCriteria(ttc => GetAugmentedHeroTurnTakers().Contains(ttc)),
                numberOfCards: drawNumeral, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator routine;
            List<SelectTurnTakerDecision> storedHero;
            List<DestroyCardAction> storedDestroy;
            LinqTurnTakerCriteria heroCriteria;

            switch (index)
            {
                case 0:

                    // One player may destroy 1 of their ongoing cards to draw 3 cards.

                    storedDestroy = new List<DestroyCardAction> { };
                    storedHero = new List<SelectTurnTakerDecision> { };
                    heroCriteria = new LinqTurnTakerCriteria(tt => tt.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsOngoing(c)).Any());

                    routine = GameController.SelectHeroToDestroyTheirCard(DecisionMaker, (httc) => new LinqCardCriteria(c => c.Owner == httc.TurnTaker && c.IsInPlayAndHasGameText && IsOngoing(c), "ongoing"),
                                    additionalCriteria: heroCriteria,
                                    storedResultsTurnTaker: storedHero,
                                    storedResultsAction: storedDestroy,
                                    cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    if (DidDestroyCard(storedDestroy))
                    {

                        HeroTurnTakerController httc = base.FindHeroTurnTakerController(GetSelectedTurnTaker(storedHero).ToHero());

                        routine = GameController.DrawCards(httc, Incapacitate1CardsToDraw, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }
                    }

                    break;

                case 1:

                    // One player may destroy 1 of their equipment cards to play 3 cards.
                    storedDestroy = new List<DestroyCardAction> { };
                    storedHero = new List<SelectTurnTakerDecision> { };
                    heroCriteria = new LinqTurnTakerCriteria(tt => tt.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsEquipment(c)).Any());

                    routine = GameController.SelectHeroToDestroyTheirCard(DecisionMaker, (httc) => new LinqCardCriteria(c => c.Owner == httc.TurnTaker && c.IsInPlayAndHasGameText && IsEquipment(c), "equipment"),
                                additionalCriteria: heroCriteria,
                                storedResultsTurnTaker: storedHero,
                                storedResultsAction: storedDestroy,
                                cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    if (DidDestroyCard(storedDestroy))
                    {

                        HeroTurnTakerController httc = base.FindHeroTurnTakerController(GetSelectedTurnTaker(storedHero).ToHero());

                        routine = GameController.SelectAndPlayCardsFromHand(httc, Incapacitate2CardsToPlay, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }
                    }

                    break;

                case 2:

                    // One target regains 1 HP.
                    routine = base.GameController.SelectAndGainHP(this.HeroTurnTakerController, Incapacitate3HpGain,
                        cardSource: GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;
            }
        }
    }
}