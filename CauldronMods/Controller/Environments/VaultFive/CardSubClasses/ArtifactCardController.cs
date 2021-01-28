using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.VaultFive
{
    public class ArtifactCardController : VaultFiveUtilityCardController
    {

        public ArtifactCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildArtifactSpecialString(), relatedCards: () => Card.Owner.CharacterCards).Condition = () => Card.Owner != TurnTaker && Card.IsInDeck;
        }

        private string BuildArtifactSpecialString()
        {
            if(Card.Owner == TurnTaker || !Card.IsInDeck)
            {
                return "";
            }

            IEnumerable<Card> artifactsInDeck = FindCardsWhere(c => c.Location == Card.Location && c.ParentDeck == Card.ParentDeck);
            List<int> positionList = new List<int>();
            int position;
            foreach(Card artifact in artifactsInDeck)
            {
                positionList.Add(Card.Location.Cards.Reverse().ToList().IndexOf(artifact) + 1);
            }
            positionList.Sort();
            string positionString = "An artifact is the ";
            for(int i=0; i < positionList.Count; i++)
            {
                position = positionList.ElementAt(i);
                positionString += position;
                if (position == 1)
                {
                    positionString += "st";
                }
                else if (position == 2)
                {
                    positionString += "nd";
                }
                else if (position == 3)
                {
                    positionString += "rd";
                }
                else
                {
                    positionString += "th";
                }

                if(i < positionList.Count - 1 && positionList.Count > 2)
                {
                    positionString += ", ";
                } else if(i < positionList.Count - 1 && positionList.Count == 2)
                {
                    positionString += " ";
                }
                if(i == positionList.Count - 2)
                {
                    positionString += "and ";
                }
            }
            
            positionString += " card in " + Card.Owner.Name + "'s deck.";

            return positionString;
        }

        private const string  FirstTimeEnteredPlay = "FirstTimeEnteredPlay";

        public virtual IEnumerator UniqueOnPlayEffect() { return null; }

        private bool HasOwner
        {
            get
            {
                return GetCardPropertyJournalEntryBoolean(FirstTimeEnteredPlay) != null && HasBeenSetToTrueThisGame(FirstTimeEnteredPlay);
            }
        }
        public override IEnumerator Play()
        {
            //The first time this card enters play, select a player. Treat this card as part of their deck for the rest of the game.
            if(!HasOwner)
            {
                SetCardPropertyToTrueIfRealAction(FirstTimeEnteredPlay);

                List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.TurnTaker, false, false, storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectTurnTaker(storedResults))
                {
                    GameController.AddCardPropertyJournalEntry(Card, "OverrideTurnTaker", new string[] { "Cauldron.VaultFive", Card.Identifier });
                    TurnTaker hero = GetSelectedTurnTaker(storedResults);
                    GameController.ChangeCardOwnership(Card, hero);
                    coroutine = GameController.SendMessageAction($"{Card.Title} is now a part of { hero.ShortName}'s deck!", Priority.High, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            //Do the unique effect for this artifact
            IEnumerator coroutine2 = UniqueOnPlayEffect();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine2);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine2);
            }

            //Then move this card beneath the top 2 cards of its deck face down.
            IEnumerator coroutine3 = GameController.MoveCard(TurnTakerController, Card, Card.NativeDeck, offset: 2, showMessage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine3);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine3);
            }

            yield break;

        }

        protected IEnumerator SelectActiveHeroCharacterCardToDoAction(List<Card> storedResults, SelectionType selectionType)
        {
            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, selectionType, new LinqCardCriteria((Card c) => c.Owner == Card.Owner && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active hero"), storedDecision, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
            if (selectCardDecision != null)
            {
                storedResults.Add(selectCardDecision.SelectedCard);
            }

            yield break;
        }
    }
}