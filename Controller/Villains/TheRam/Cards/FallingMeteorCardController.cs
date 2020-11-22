using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace Cauldron.TheRam
{
    public class FallingMeteorCardController : TheRamUtilityCardController
    {
        public FallingMeteorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Search the villain deck and trash for {H - 2} copies of Up Close and play them next to the {H - 2} heroes with the highest HP that are not up close. If you searched the deck, shuffle it. 
            List<bool> searchDeckFirst = new List<bool> { };
            List<Card> foundCards = new List<Card> { };
            IEnumerator playUpClose;
            for(int i = 0; i < H-2; i++)
            {
                playUpClose = PlayUpCloseFromDeckOrTrash(searchDeckFirst, (H-2) - i, foundCards);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playUpClose);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playUpClose);
                }
                if (foundCards.Count() != i + 1)
                {
                    //no more cards to find, stop looking
                    i += H;
                }
            }

            //"{TheRam} deals each non-villain target {H} projectile damage."
            IEnumerator damage = DealDamage(GetRam, (Card c) => c.IsInPlayAndHasGameText && c.IsNonVillainTarget, H, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damage);
            }
            yield break;
        }

        private IEnumerator PlayUpCloseFromDeckOrTrash(List<bool> searchDeckFirst, int targetsLeft, List<Card> foundCards)
        {
            bool knowDeckOrTrash = searchDeckFirst.Count() > 0;
            //if we have already decided whether to search the trash or the deck, don't search the other if possible
            bool willAvoidSearchingTrash = TurnTaker.GetCardsWhere((Card c) => c.IsInDeck && c.Identifier == "UpClose").Any() && knowDeckOrTrash && searchDeckFirst.FirstOrDefault() == false;
            bool willAvoidSearchingDeck = TurnTaker.GetCardsWhere((Card c) => c.IsInTrash && c.Identifier == "UpClose").Any() && knowDeckOrTrash && searchDeckFirst.FirstOrDefault() == true;

            //TODO - this bit above is not working! figure out what mistake i made.
            //it will probably be very simple and embarassing in retrospect

            List<Function> actions = new List<Function> { };
            actions.Add(new Function(DecisionMaker,
                               "Search trash first.",
                               SelectionType.SearchTrash,
                               () => SearchSelectHeroAndPlayUpClose(false, targetsLeft, foundCards),
                               !willAvoidSearchingTrash));
            actions.Add(new Function(DecisionMaker,
                                "Search deck first.",
                                SelectionType.SearchDeck,
                                () => SearchSelectHeroAndPlayUpClose(true, targetsLeft, foundCards),
                                !willAvoidSearchingDeck));

            Log.Debug($"willAvoidSearchingTrash is {willAvoidSearchingTrash}, willAvoidSearchingDeck is {willAvoidSearchingDeck}");

            List<SelectFunctionDecision> deckOrTrashFirst = new List<SelectFunctionDecision> { };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, actions, false, null, "There were no more copies of Up Close to find.", null, GetCardSource());
            IEnumerator decision = GameController.SelectAndPerformFunction(selectFunction, deckOrTrashFirst);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(decision);
            }
            else
            {
                base.GameController.ExhaustCoroutine(decision);
            }

            if(deckOrTrashFirst.FirstOrDefault().Index == 1)
            {
                var shuffleAction = new ShuffleCardsAction(new CardSource(CharacterCardController), this.TurnTaker.Deck);
                IEnumerator shuffle = GameController.DoAction(shuffleAction);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(shuffle);
                }
                else
                {
                    GameController.ExhaustCoroutine(shuffle);
                }
            }

            if(!knowDeckOrTrash)
            {
                searchDeckFirst.Add(deckOrTrashFirst.FirstOrDefault().Index == 1);
            }
            yield break;
        }

        private IEnumerator SearchSelectHeroAndPlayUpClose(bool trySearchDeck, int targetsLeft, List<Card> foundCard)
        {
            Location searchLoc; 
            if (trySearchDeck && TurnTaker.GetCardsWhere((Card c) => c.IsInDeck && c.Identifier == "UpClose").Any())
            {
                searchLoc = TurnTaker.Deck;
            }
            else
            {
                searchLoc = TurnTaker.Trash;
            }

            Card close = GameController.FindCardsWhere((Card c) => searchLoc.Cards.Contains(c) && c.Identifier == "UpClose").FirstOrDefault();

            if (close == null)
            {
                IEnumerator message = GameController.SendMessageAction("There were no more copies of Up Close to find. (sub-option)", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            foundCard.Add(close);

            List<SelectCardDecision> selectedHero = new List<SelectCardDecision> { };

            //select, from the non-Up Close heroes, one target that could be among the highest targetsLeft
            List<Card> highestOptions = GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && !IsUpClose(c), GetCardSource(), targetsLeft).ToList();
            Log.Debug($"Found {highestOptions.Count()} choices: ");
            foreach (Card c in highestOptions)
            {
                Log.Debug(c.Title);
            }
            IEnumerator selectHero = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroCharacterCard, highestOptions, selectedHero, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectHero);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectHero);
            }
            IEnumerator playCard;

            if (DidSelectCard(selectedHero))
            {
                Log.Debug("Automatically playing");
                playCard = PlayGivenUpCloseByGivenCard(close, selectedHero.FirstOrDefault().SelectedCard, isPutIntoPlay: false);
            }
            else
            {
                Log.Debug("Could not autoplay.");
                playCard = GameController.PlayCard(DecisionMaker, close, cardSource: GetCardSource());
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCard);
            }

            yield break;
        }
    }
}