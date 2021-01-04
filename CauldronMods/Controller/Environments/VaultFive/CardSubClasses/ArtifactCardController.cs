using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.VaultFive
{
    public class ArtifactCardController : VaultFiveUtilityCardController
    {

        public ArtifactCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private const string  FirstTimeEnteredPlay = "FirstTimeEnteredPlay";

        public virtual IEnumerator UniqueOnPlayEffect() { return null; }
        public override IEnumerator Play()
        {
            //The first time this card enters play, select a player. Treat this card as part of their deck for the rest of the game.
            if(!HasBeenSetToTrueThisGame(FirstTimeEnteredPlay))
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
                    TurnTaker hero = GetSelectedTurnTaker(storedResults);
                    this.Card.SetNewOwner(hero);
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