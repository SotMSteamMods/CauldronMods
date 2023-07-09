using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{

    public class AceOfSorrowsBaccaratCharacterCardController : HeroCharacterCardController
    {
        public AceOfSorrowsBaccaratCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick") && c.Location == base.HeroTurnTaker.Trash, "tricks in trash"));
        }

        private List<Card> actedHeroes;

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may shuffle a card from their trash into their deck, then put all cards with the same name from their trash into their hand.
                        List<SelectTurnTakerDecision> selectTurnTakerDecision = new List<SelectTurnTakerDecision>();
                        //One player...
                        IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(base.HeroTurnTakerController, SelectionType.MoveCard, false, false, selectTurnTakerDecision, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if(!DidSelectTurnTaker(selectTurnTakerDecision))
                        {
                            yield break;
                        }
                        TurnTaker selectedTurnTaker = GetSelectedTurnTaker(selectTurnTakerDecision);
                        List<SelectCardDecision> selectCardDecision = new List<SelectCardDecision>();
                        //...may shuffle a card from their trash into their deck...
                        coroutine = base.GameController.SelectCardAndStoreResults(base.GameController.FindHeroTurnTakerController(selectedTurnTaker.ToHero()), SelectionType.ShuffleCardFromTrashIntoDeck, new LinqCardCriteria((Card c) => c.Location == selectedTurnTaker.Trash), selectCardDecision, true, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectCard(selectCardDecision))
                        {
                            Card selectedCard = selectCardDecision.FirstOrDefault().SelectedCard;
                            coroutine = base.GameController.ShuffleCardsIntoLocation(base.GameController.FindHeroTurnTakerController(selectedTurnTaker.ToHero()), selectedCard.ToEnumerable<Card>(), selectedTurnTaker.Deck, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            //...then put all cards with the same name from their trash into their hand.
                            IEnumerable<Card> cardsToMove = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location == selectedTurnTaker.Trash && c.Title == selectedCard.Title));
                            MoveCardDestination selectedHand = new MoveCardDestination(selectedTurnTaker.ToHero().Hand);
                            coroutine = base.GameController.MoveCards(base.GameController.FindHeroTurnTakerController(selectedTurnTaker.ToHero()), cardsToMove, selectedTurnTaker.ToHero().Hand, cardSource: base.GetCardSource());
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
                case 1:
                    {
                        //Each target regains 1 HP.
                        IEnumerator coroutine2 = base.GameController.GainHP(base.HeroTurnTakerController, (Card c) => c.IsTarget, 1, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //Each hero character may deal themselves 3 toxic damage to play a card now.
                        this.actedHeroes = new List<Card>();
                        IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
                        {
                            new Function(base.FindCardController(c).DecisionMaker, "Deal self 3 toxic damage to play a card now.", SelectionType.PlayCard, () => this.DealDamageAndDrawResponse(c))
                        };
                        IEnumerator coroutine3 = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }
        private IEnumerator DealDamageAndDrawResponse(Card card)
        {
            if (card != null)
            {
                IEnumerator coroutine = base.DealDamage(card, card, 3, DamageType.Toxic, cardSource: base.GetCardSource());
                IEnumerator coroutine2 = base.GameController.SelectAndPlayCardFromHand(base.GameController.FindHeroTurnTakerController(card.Owner.ToHero()), false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
                this.LogActedCard(card);
            }
            yield break;
        }

        private void LogActedCard(Card card)
        {
            if (card.SharedIdentifier != null)
            {
                IEnumerable<Card> collection = base.FindCardsWhere((Card c) => c.SharedIdentifier != null && c.SharedIdentifier == card.SharedIdentifier && c != card, false, null, false);
                this.actedHeroes.AddRange(collection);

            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int trashedTricks = base.GetPowerNumeral(0, 2);
            MoveCardDestination hand = new MoveCardDestination(base.HeroTurnTaker.Hand);
            //Play any number of tricks from your hand, or put 2 tricks from your trash into your hand.
            IEnumerable<Function> functionChoices = new Function[]
            { 
                //Play any number of tricks from your hand
                new Function(base.HeroTurnTakerController, "Play any number of tricks from your hand", SelectionType.PlayCard, () => base.SelectAndPlayCardsFromHand(base.HeroTurnTakerController, 40, true, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick")))),
                //put 2 tricks from your trash into your hand
                new Function(base.HeroTurnTakerController, "put 2 tricks from your trash into your hand", SelectionType.MoveCardToHandFromTrash, () => base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController, base.TurnTaker.Trash, trashedTricks, trashedTricks, new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick")), hand.ToEnumerable<MoveCardDestination>(), cardSource: base.GetCardSource()))
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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