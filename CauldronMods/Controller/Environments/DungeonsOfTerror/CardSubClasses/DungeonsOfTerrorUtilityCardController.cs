using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public abstract class DungeonsOfTerrorUtilityCardController : CardController
    {
        protected DungeonsOfTerrorUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string FateKeyword = "fate";
        public static readonly string RingOfForesightIdentifier = "RingOfForesight";


        protected bool IsFate(Card card)
        {
            return card.DoKeywordsContain(FateKeyword);
        }

        private IEnumerable<Card> FindRingOfForesight()
        {
            return base.FindCardsWhere(c => c.Identifier == RingOfForesightIdentifier);
        }

        protected bool IsRingOfForesightInPlay()
        {
            return FindRingOfForesight().Where(c => c.IsInPlayAndHasGameText).Any();
        }

        protected IEnumerator CheckForNumberOfFates(IEnumerable<Card> cardsToCheck, List<int> storedResults, Location checkingLocation = null, List<bool> suppressMessage = null)
        { 
            int numFates = 0;
            if(!cardsToCheck.Any(c => c != null))
            {
                yield break;
            }
            if(checkingLocation != null && checkingLocation == TurnTaker.Trash)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Checking if the top card of the environment trash is a fate card!", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (IsRingOfForesightInPlay())
                {
                    //When checking a card in the environment trash, the players may first destroy ring of foresight, and then check it instead of the original card.
                    Card ring = FindRingOfForesight().First();
                    CardSource ringSource = FindCardController(ring).GetCardSource();
                    List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();
                    coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, ring, storedResults: storedYesNo, cardSource: ringSource);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (DidPlayerAnswerYes(storedYesNo))
                    {
                        coroutine = GameController.DestroyCard(DecisionMaker, ring, cardSource: ringSource);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        coroutine = GameController.SendMessageAction($"Checking if {ring.Title} is a fate card instead of the top card of {TurnTaker.Trash.GetFriendlyName()}", Priority.High, GetCardSource(), associatedCards: ring.ToEnumerable());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        cardsToCheck = ring.ToEnumerable();
                        suppressMessage?.Add(true);
                    }
                    else
                    {
                        suppressMessage?.Add(false);
                    }

                }
            }

            

            
            foreach (Card card in cardsToCheck)
            {
                if(card != null && IsFate(card))
                {
                    numFates++;
                }
            }
            storedResults.Add(numFates);
            yield break;
        }

        protected bool? IsTopCardOfLocationFate(Location location)
        {
            Card card = location.TopCard;
            if (card == null)
            {
                return null;
            }

            return IsFate(card);
        }

        protected string BuildTopCardOfLocationSpecialString(Location location)
        {
            bool? fate = IsTopCardOfLocationFate(location);
            string special = "";
            if(fate == null)
            {
                special = $"There are no cards in {location.GetFriendlyName()}.";
            } else

            {
                special = $"The top card of {location.GetFriendlyName()} is ";
                if(fate.Value == false)
                {
                    special += "not ";
                }
                special += "a fate card.";
            }

            return special;
        }
    }
}
