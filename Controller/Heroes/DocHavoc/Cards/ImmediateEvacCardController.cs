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
        private List<Card> actedHeroes = new List<Card>();


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


            Func<HeroTurnTakerController, IEnumerable<Function>> functions
                = new Func<HeroTurnTakerController, IEnumerable<Function>>(
                    h => new List<Function>()
                    {
                        new Function(h, "Take a card from your trash and place it in your hand", SelectionType.DiscardAndDrawCard,
                            (()=> TakeCardFromTrashResponse(h.CharacterCard))),
                        new Function(h, "Discard a card and draw 2 cards", SelectionType.DiscardAndDrawCard, 
                            (() => DiscardCardAndDrawCardsResponse(h.CharacterCard)))
                    });

            IEnumerator coroutine2 = this.EachPlayerSelectsFunction(
                (Func<HeroTurnTakerController, bool>) (h => h.IsHero),
                functions,
                outputIfCannotChooseFunction: ((Func<HeroTurnTakerController, string>)(h => h.Name + " no choices")));

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine2);
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
            if (card != null)
            {
                List<SelectCardDecision> selectCardDecision = new List<SelectCardDecision>();

                IEnumerator routine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, 
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


                routine = base.GameController.MoveCard(this.TurnTakerController, selectCardDecision.First().SelectedCard,
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
        }
        
        private IEnumerator DiscardCardAndDrawCardsResponse(Card card)
        {
            if (card != null)
            {
                List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
                IEnumerator discardCardRoutine
                    = this.GameController.EachPlayerDiscardsCards(1, 1, storedResults);

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
                    IEnumerator drawCardsRoutine = this.DrawCards(dca.HeroTurnTakerController, 2);
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

        private void LogActedCard(Card card)
        {
            if (card.SharedIdentifier == null)
                return;
            this.actedHeroes.AddRange(this.FindCardsWhere((Func<Card, bool>)(c => c.SharedIdentifier != null && c.SharedIdentifier == card.SharedIdentifier && c != card)));
        }
    }
}
