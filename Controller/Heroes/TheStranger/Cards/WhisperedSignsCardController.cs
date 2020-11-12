using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class WhisperedSignsCardController : CardController
    {
        #region Constructors

        public WhisperedSignsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //You may draw 2 cards or put a Rune from your trash into your hand.            
            string option1 = "Draw 2 cards";
            string option2 = "Put a Rune from your trash into your hand";
            List<Function> list = new List<Function>();
            list.Add(new Function(this.DecisionMaker, option1, SelectionType.DrawCard, () => base.DrawCards(this.DecisionMaker,2), new bool?(true), null, option1));
            list.Add(new Function(this.DecisionMaker, option2, SelectionType.MoveCardToHandFromTrash, () => base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => this.IsRune(c), "rune"), new MoveCardDestination[] { new MoveCardDestination(base.Card.Owner.ToHero().Hand, false, false, false) }),null, null, option2));
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

            //You may play a Rune or Glyph now.
            IEnumerator play = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, new LinqCardCriteria((Card card) => this.IsRune(card) || this.IsGlyph(card), "rune or glyph"), false, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(play);
            }
            else
            {
                base.GameController.ExhaustCoroutine(play);
            }
            yield break;
        }
        private bool IsGlyph(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "glyph", false, false);
        }

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }

        #endregion Methods
    }
}