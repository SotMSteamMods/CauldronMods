using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Cauldron.Baccarat
{

    public class AceOfSwordsBaccaratCharacterCardController : HeroCharacterCardController
    {
        public AceOfSwordsBaccaratCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private List<Card> actedHeroes;

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //Select a card in a trash. Destroy a card in play with the same name.
                        SelectCardDecision selectDecision = new SelectCardDecision(GameController, base.HeroTurnTakerController, SelectionType.DestroyCard, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash)), cardSource: base.GetCardSource());
                        IEnumerator coroutine = base.GameController.SelectCardAndDoAction(selectDecision, (SelectCardDecision decision) => base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => decision.SelectedCard.Identifier == c.Identifier && c.IsInPlayAndHasGameText, "selected card"), false, cardSource: base.GetCardSource()));
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
                case 1:
                    {
                        //One target deals itself 1 energy damage.
                        SelectCardDecision selectDecision = new SelectCardDecision(GameController, base.HeroTurnTakerController, SelectionType.DealDamage, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget)), cardSource: base.GetCardSource());
                        IEnumerator coroutine2 = base.GameController.SelectCardAndDoAction(selectDecision, (SelectCardDecision decision) => base.DealDamage(decision.SelectedCard, decision.SelectedCard, 1, DamageType.Energy, cardSource: base.GetCardSource()));
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
                        //Each hero character may deal themselves 1 toxic damage to draw a card now.
                        this.actedHeroes = new List<Card>();
                        IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
                        {
                            new Function(base.FindCardController(c).DecisionMaker, "Deal self 1 toxic damage to draw a card now.", SelectionType.DrawCard, () => this.DealDamageAndDrawResponse(c))
                        };
                        IEnumerator coroutine3 = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
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
                IEnumerator coroutine = base.DealDamage(card, card, 1, DamageType.Toxic, cardSource: base.GetCardSource());
                IEnumerator coroutine2 = base.DrawCard(card.Owner.ToHero());
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
                coroutine2 = null;
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
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            int X = 0;
            bool skipped = false;
            List<SelectCardDecision> decisions = new List<SelectCardDecision>();
            Card selectedCard;
            List<Card> playedCards = new List<Card>();
            int upTo = GetPowerNumeral(0, 3);
            int discardBase = GetPowerNumeral(1, 3);
            IEnumerator coroutine;
            while (X < upTo && !skipped)
            {
                //Play X copies of Afterlife Euchre from your trash (up to 3).
                coroutine = base.GameController.SelectAndMoveCard(base.HeroTurnTakerController, (Card c) => c.Identifier == "AfterlifeEuchre" && !playedCards.Contains(c) && c.Location == base.TurnTaker.Trash, base.TurnTaker.PlayArea, optional: true, playIfMovingToPlayArea: true, storedResults: decisions, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                selectedCard = decisions.LastOrDefault<SelectCardDecision>().SelectedCard;
                if (decisions.Count() > X && selectedCard != null)
                {
                    playedCards.Add(selectedCard);
                    X++;
                }
                else
                {
                    skipped = true;
                }
            }
            //Discard the top 3 - X cards of your deck.
            coroutine = base.DiscardCardsFromTopOfDeck(base.TurnTakerController, discardBase - X);
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