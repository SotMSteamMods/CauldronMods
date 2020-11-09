using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static string Identifier = "ImmediateEvac";
        
        private const int HpGain = 2;
        private const int CardsToDiscard = 1;
        private const int CardsToDrawFromDeck = 2;

        private const string ChoiceTextSelectTrashIntoHand = "Take a card from your trash and place it in your hand";
        private const string ChoiceTextDiscardAndDraw = "Discard a card and draw 2 cards";

        public ImmediateEvacCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {

            //Func<Card, IEnumerable<Function>> functionsBasedOnCard = new Func<Card, IEnumerable<Function>>(this.DiscardCardAndDrawCardsResponse);

            /*
            Func<HeroTurnTakerController, IEnumerable<Function>> functions 
                = new Func<HeroTurnTakerController, IEnumerable<Function>>(h => h.GetCardsAtLocation);

            IEnumerator coroutine = this.EachPlayerSelectsFunction((Func<HeroTurnTakerController, bool>)
                (h => !h.IsIncapacitatedOrOutOfGame), functions, new int?(0), 
                outputIfCannotChooseFunction: ((Func<HeroTurnTakerController, string>)(h 
                    => h.Name + " has no cards in their trash or their deck.")));
            */


            /*
            Func<Card, IEnumerable<Function>> functionsBasedOnCard 
                = new Func<Card, IEnumerable<Function>>(c => DiscardCardAndDrawCardsResponse(c));

            IEnumerator coroutine2 = this.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, 
                new LinqCardCriteria(new Func<Card, bool>(c => c.IsHero && c.IsActive), "active hero character cards", false), 
                functionsBasedOnCard, true, this.GetCardSource());
            */


            IEnumerable<Function> Functions(HeroTurnTakerController h) => new List<Function>()
            {
                new Function(h, ChoiceTextSelectTrashIntoHand, SelectionType.MoveCardToHandFromTrash, () => TakeCardFromTrashResponse(h.CharacterCard)), 
                new Function(h, ChoiceTextDiscardAndDraw, SelectionType.DiscardAndDrawCard, () => DiscardCardAndDrawCardsResponse(h.CharacterCard))
            };

            List<SelectFunctionDecision> choicesMade = new List<SelectFunctionDecision>();
            IEnumerator playerSelectRoutine = this.EachPlayerSelectsFunction(
                (Func<HeroTurnTakerController, bool>) (h => h.IsHero && !h.IsIncapacitatedOrOutOfGame),
                Functions,
                storedResults: choicesMade,
                outputIfCannotChooseFunction: ((Func<HeroTurnTakerController, string>)(h => $"{h.Name} has no valid choices")));

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
                (Func<Card, bool>)(c => c.IsVillainTarget), HpGain,
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

        private IEnumerator TakeCardFromTrashResponse(Card card)
        {
            if (card == null)
            {
                yield break;
            }

            HeroTurnTakerController heroTurnTakerController =
                this.GameController.HeroTurnTakerControllers.First(httc => httc.CharacterCard.Equals(card));

            List <SelectCardDecision> selectCardDecision = new List<SelectCardDecision>();

            IEnumerator routine = base.GameController.SelectCardAndStoreResults(heroTurnTakerController, 
                SelectionType.MoveCardToHand, 
                new LinqCardCriteria((Card c) => c.IsInTrash && this.GameController.IsLocationVisibleToSource(c.Location, base.GetCardSource(null))), 
                selectCardDecision, false);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!selectCardDecision.Any())
            {
                yield break;
            }

            routine = base.GameController.MoveCard(heroTurnTakerController, selectCardDecision.First().SelectedCard,
                new Location(selectCardDecision.First().SelectedCard, LocationName.Hand));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
        
        private IEnumerator DiscardCardAndDrawCardsResponse(Card card)
        {
            if (card == null)
            {
                yield break;
            }

            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCardRoutine
                = this.GameController.EachPlayerDiscardsCards(CardsToDiscard, CardsToDiscard, storedResults);

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
