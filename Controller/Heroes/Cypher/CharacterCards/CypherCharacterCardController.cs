using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cypher
{
    public class CypherCharacterCardController : CypherBaseCharacterCardController
    {

        private const int PowerCardsToDraw = 1;
        private const int Incapacitate1OngoingToDestroy = 1;
        private const int Incapacitate1CardsToDraw = 3;
        private const int Incapacitate2EquipmentToDestroy = 1;
        private const int Incapacitate2CardsToDraw = 3;
        private const int Incapacitate3HpGain = 1;

        public CypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

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
            List<SelectCardDecision> storedHero;
            LinqCardCriteria criteria;

            switch (index)
            {
                case 0:

                    // One player may destroy 1 of their ongoing cards to draw 3 cards.
                    storedHero = new List<SelectCardDecision>();
                    criteria = new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero", false);
                    routine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DestroyCard, criteria, storedHero, false, cardSource: base.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    if (DidSelectCard(storedHero))
                    {
                        // Ask hero if they want to destroy one of their ongoings
                        Card heroCard = GetSelectedCard(storedHero);
                        HeroTurnTakerController httc = base.FindHeroTurnTakerController(heroCard.Owner.ToHero());

                        List<DestroyCardAction> dcas = new List<DestroyCardAction>();
                        routine = base.GameController.SelectAndDestroyCard(httc,
                            new LinqCardCriteria(c => c.IsOngoing), false, dcas, cardSource: GetCardSource());

                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }

                        if (dcas.Any() && dcas.First().WasCardDestroyed)
                        {
                            routine = base.GameController.DrawCards(httc, Incapacitate1CardsToDraw);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(routine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(routine);
                            }
                        }
                    }

                    break;

                case 1:
                    
                    // One player may destroy 1 of their equipment cards to play 3 cards.
                    storedHero = new List<SelectCardDecision>();
                    criteria = new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero", false);
                    routine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DestroyCard, criteria, storedHero, false, cardSource: base.GetCardSource());
                    
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    if (DidSelectCard(storedHero))
                    {
                        // Ask hero if they want to destroy one of their equipment
                        Card heroCard = GetSelectedCard(storedHero);
                        HeroTurnTakerController httc = base.FindHeroTurnTakerController(heroCard.Owner.ToHero());

                        List<DestroyCardAction> dcas = new List<DestroyCardAction>();
                        routine = base.GameController.SelectAndDestroyCard(httc,
                            new LinqCardCriteria(c => IsEquipment(c)), false, dcas, cardSource: GetCardSource());

                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }

                        if (dcas.Any() && dcas.First().WasCardDestroyed)
                        {
                            routine = base.GameController.DrawCards(httc, Incapacitate2CardsToDraw);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(routine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(routine);
                            }
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