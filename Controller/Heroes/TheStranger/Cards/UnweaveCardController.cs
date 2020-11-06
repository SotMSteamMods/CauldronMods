using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class UnweaveCardController : CardController
    {
        #region Constructors

        public UnweaveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //You may shuffle up to 4 Runes from your trash into your deck, or discard up to 4 cards.
           
            //shuffle card function details
            List<MoveCardDestination> deck = new List<MoveCardDestination>();
            deck.Add(new MoveCardDestination(base.HeroTurnTaker.Deck, false, false, false));
            List<MoveCardAction> storedShuffle = new List<MoveCardAction>();
            Func<IEnumerator> shuffleFunc = new Func<IEnumerator>( () => base.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, base.TurnTaker.Trash, new int?(0), 4, new LinqCardCriteria((Card c) => this.IsRune(c), "rune", true, false, null, null, false), deck, false, true, true, false, null, storedShuffle, false, false, false, null, false, true, null, null, base.GetCardSource(null)));
            string shuffleCardsMessage = "You may shuffle up to 4 Runes from your trash into your deck";
            Function shuffle = new Function(this.DecisionMaker, shuffleCardsMessage, SelectionType.ShuffleCardFromTrashIntoDeck, shuffleFunc, null, null, shuffleCardsMessage);
            //discard card function details
            List<DiscardCardAction> storedDiscard = new List<DiscardCardAction>();
            Func<IEnumerator> discardFunc = new Func<IEnumerator>(() => base.SelectAndDiscardCards(this.DecisionMaker, new int?(4), false, new int?(0), storedDiscard, false, null, null, null, SelectionType.DiscardCard, base.TurnTaker));
            string discardCardsMessage = "You may discard up to 4 cards";
            Function discard = new Function(this.DecisionMaker, discardCardsMessage, SelectionType.DiscardCard, discardFunc, null, null, discardCardsMessage);
           
            //add both to a list and choose which one to use    
            List<Function> list = new List<Function>();
            list.Add(shuffle);
            list.Add(discard);

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, null, null, null, base.GetCardSource(null));
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //For each card shuffled or discarded this way, {TheStranger} may draw a card or regain 1HP.
            //get the number of actions taken
            int numberOfCardsDiscarded = base.GetNumberOfCardsDiscarded(storedDiscard);
            int numberOfCardsShuffled = base.GetNumberOfCardsMoved(storedShuffle);
            int maxActions = Math.Max(numberOfCardsDiscarded, numberOfCardsShuffled);

            //define function options
            Func<IEnumerator> draw = new Func<IEnumerator>(() => base.DrawCard(base.HeroTurnTaker, true, null, true));
            Func<IEnumerator> gainHp = new Func<IEnumerator>(() => base.GameController.GainHP(base.CharacterCard, new int?(1)));
            string drawCardsMessage = "You may draw a card";
            string gainHpMessage = "Regain 1 HP";
            List<Function> list2 = new List<Function>();
            list2.Add(new Function(this.DecisionMaker, drawCardsMessage, SelectionType.DrawCard, draw, null, null, drawCardsMessage));
            list2.Add(new Function(this.DecisionMaker, gainHpMessage, SelectionType.GainHP, gainHp, null, null, gainHpMessage));
           
            //ask the player repeatedly if they want to take their actions
            for (int i = 0; i < maxActions; i++)
            {
                SelectFunctionDecision selectFunction2 = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list2, false, null, null, null, base.GetCardSource(null));
                IEnumerator coroutine2 = base.GameController.SelectAndPerformFunction(selectFunction2, null, null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            yield break;
        }

        

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }
        #endregion Methods
    }
}