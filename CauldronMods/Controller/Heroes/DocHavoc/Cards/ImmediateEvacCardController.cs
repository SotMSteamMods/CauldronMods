using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class ImmediateEvacCardController : CardController
    {
        //==============================================================
        // Each player may take a card from their trash and place it in their hand, or discard a card and draw 2 cards.
        // Each villain target regains 2HP.
        //==============================================================

        public static readonly string Identifier = "ImmediateEvac";

        private const int HpGain = 2;
        private const int CardsToDrawFromDeck = 2;

        private const string ChoiceTextSelectTrashIntoHand = "Take a card from your trash and place it in your hand";
        private const string ChoiceTextDiscardAndDraw = "Discard a card and draw 2 cards";

        public ImmediateEvacCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerable<Function> Functions(HeroTurnTakerController h) => new List<Function>()
            {
                new Function(h, ChoiceTextSelectTrashIntoHand, SelectionType.MoveCardToHandFromTrash,
                    () => base.GameController.SelectCardFromLocationAndMoveIt(h, h.TurnTaker.Trash,
                        new LinqCardCriteria((Card c) => c.Location.Equals(h.TurnTaker.Trash)),
                        new MoveCardDestination[] { new MoveCardDestination(h.HeroTurnTaker.Hand) }),
                    repeatDecisionText: ChoiceTextSelectTrashIntoHand),

                new Function(h, ChoiceTextDiscardAndDraw, SelectionType.DiscardAndDrawCard, () => DiscardCardAndDrawCardsResponse(h)),
            };

            List<SelectFunctionDecision> choicesMade = new List<SelectFunctionDecision>();
            IEnumerator playerSelectRoutine = this.EachPlayerSelectsFunction(
                (h => !h.IsIncapacitatedOrOutOfGame),
                Functions,
                storedResults: choicesMade,
                outputIfCannotChooseFunction: (h => $"{h.Name} has no valid choices"));


            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(playerSelectRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(playerSelectRoutine);
            }

            // Villain targets gain 2 HP
            IEnumerator gainHpRoutine = this.GameController.GainHP(this.HeroTurnTakerController,
                (Func<Card, bool>)(c => IsVillainTarget(c)), HpGain,
                cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(gainHpRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(gainHpRoutine);
            }
        }


        private IEnumerator DiscardCardAndDrawCardsResponse(HeroTurnTakerController httc)
        {
            if (httc is null)
            {
                yield break;
            }

            //discard a card and draw 2 cards
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();

            IEnumerator discardCardRoutine = this.GameController.SelectAndDiscardCard(httc, storedResults: storedResults, cardSource: GetCardSource());


            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            foreach (DiscardCardAction dca in storedResults.Where(dca => dca.WasCardDiscarded))
            {
                IEnumerator drawCardsRoutine = this.DrawCards(dca.HeroTurnTakerController, CardsToDrawFromDeck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(drawCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(drawCardsRoutine);
                }
            }
        }
    }
}
