using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Handelabra;

namespace Cauldron.SuperstormAkela
{
    public class SuperstormAkelaCardController : CardController
    {

        public SuperstormAkelaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected IEnumerator MoveCardToFarLeft(Card card, bool noMessage = false)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            list.Remove(card);
            list.Insert(0, card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });

            if (!noMessage)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Moved " + card.Title + " to the far left of the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " was moved to the far left of the environment's play area.");
            }

            IEnumerator coroutine2 = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }

        protected IEnumerator MoveCardIntoFarLeft(Card card, bool noMessage = false)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            list.Insert(0, card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });

            if (!noMessage)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Played " + card.Title + " to the far left of the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " was played to the far left of the environment's play area.");
            }

            IEnumerator coroutine2 = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }



        protected IEnumerator MoveCardToFarRight(Card card, bool noMessage = false)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            list.Remove(card);
            list.Add(card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });

            if (!noMessage)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Moved " + card.Title + " to the far right of the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " was moved to the far right of the environment's play area.");
            }

            IEnumerator coroutine2 = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        protected IEnumerator MoveCardOneToTheRight(Card card, bool noMessage = false)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            if (list.Last() == card)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Can't move " + card.Title + " any farther right in the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " can't be move any farther right in the environment's play area.");

                yield break;

            }

            int index = list.IndexOf(card);
            list.Remove(card);
            list.Insert(index + 1, card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });

            if (!noMessage)
            {
                IEnumerator coroutine = GameController.SendMessageAction("Moved " + card.Title + " one space to the right in the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " was moved one space to the right in the environment's play area.");
            }

            IEnumerator coroutine2 = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        protected IEnumerator MoveToTheLeftOfCard(Card card, Card cardToMoveLeftOf)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            list.Remove(card);
            int index = list.IndexOf(cardToMoveLeftOf);
            list.Insert(index, card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });

            IEnumerator coroutine = GameController.SendMessageAction("Moved " + card.Title + " to the left of " + cardToMoveLeftOf.Title, Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Log.Debug(card.Title + " was moved to the left of " + cardToMoveLeftOf.Title + " in the environment's play area.");

            coroutine = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RefreshUI(Card card)
        {
            IEnumerator coroutine;
            IEnumerator coroutine2;

            // grab the card source for some other card visible to this card so the action can finish after initial flip
            Card otherCard = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Title != card.Title && GameController.IsCardVisibleToCardSource(c, GetCardSource())).TakeRandomFirstOrDefault(Game.RNG);
            Log.Debug($"{this.Card.Title} is using {otherCard.Title} to refresh the UI for {card.Title}");
            // if no other cards found, then we don't need to worry about refreshing the UI
            if(otherCard is null)
            {
                yield break;
            }
            CardSource otherCardSource = FindCardController(otherCard).GetCardSource();

            int? currentHP = card.IsTarget ? card.HitPoints : null;
            FlipCardAction flip1 = new FlipCardAction(otherCardSource, FindCardController(card), false, false, null);
            flip1.AllowTriggersToRespond = false;
            flip1.CanBeCancelled = false;
            FlipCardAction flip2 = new FlipCardAction(otherCardSource, FindCardController(card), false, false, null);
            flip2.AllowTriggersToRespond = false;
            flip2.CanBeCancelled = false;
            coroutine = DoAction(flip1);
            coroutine2 = DoAction(flip2);
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

            if (!(currentHP is null))
            {
                SetHPAction setHP = new SetHPAction(otherCardSource, card, currentHP.Value);
                setHP.AllowTriggersToRespond = false;
                setHP.CanBeCancelled = false;
                coroutine = DoAction(setHP);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        protected IEnumerator MoveToTheRightOfCard(Card card, Card cardToMoveRightOf)
        {
            List<Card> list = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            list.Remove(card);
            int index = list.IndexOf(cardToMoveRightOf);
            list.Insert(index + 1, card);
            list.ForEach(delegate (Card c)
            {
                base.GameController.Game.AssignPlayCardIndex(c);
            });
            IEnumerator coroutine = GameController.SendMessageAction("Moved " + card.Title + " to the right of " + cardToMoveRightOf.Title, Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Log.Debug(card.Title + " was moved to the right of " + cardToMoveRightOf.Title + " in the environment's play area.");


            IEnumerator coroutine2 = RefreshUI(card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }

        protected int? GetNumberOfCardsToTheLeftOfThisOne(Card card)
        {
            if (!TurnTaker.PlayArea.HasCard(card))
            {
                return null;
            }
            int position = FindCardPositionInLocation(TurnTaker.PlayArea, card);
            int numCardsToTheLeft = position;
            return new int?(numCardsToTheLeft);
        }

        protected bool IsLeftOfThisCard(Card card, Card thisCard)
        {
            return FindCardPositionInLocation(TurnTaker.PlayArea, card) < FindCardPositionInLocation(TurnTaker.PlayArea, thisCard);
        }

        protected bool IsRightOfThisCard(Card card, Card thisCard)
        {
            return FindCardPositionInLocation(TurnTaker.PlayArea, card) > FindCardPositionInLocation(TurnTaker.PlayArea, thisCard);
        }

        protected int? GetNumberOfCardsToTheRightOfThisOne(Card card)
        {
            if (!TurnTaker.PlayArea.HasCard(card))
            {
                return null;
            }
            int position = FindCardPositionInLocation(TurnTaker.PlayArea, card);
            List<Card> orderedPlayArea = GetOrderedCardsInLocation(TurnTaker.PlayArea).ToList();
            int numCardsToTheRight = orderedPlayArea.Count() - position - 1;
            return new int?(numCardsToTheRight);
        }

        private IEnumerable<Card> GetOrderedCardsInLocation(Location location)
        {
            return location.Cards.OrderBy((Card c) => c.PlayIndex);
        }

        private int FindCardPositionInLocation(Location location, Card c)
        {
            return GetOrderedCardsInLocation(location).ToList().IndexOf(c);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            if (playToTheLeft != null && playToTheLeft.Value)
            {
                IEnumerator coroutine = MoveCardIntoFarLeft(base.Card);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            storedResults.Add(new MoveCardDestination(TurnTaker.PlayArea));

            yield return null;
        }

        private bool? playToTheLeft
        {
            get
            {
                return Game.Journal.GetCardPropertiesBoolean(GameController.FindCard("FracturedSky"), "PlayToTheLeft");
            }
        }


        protected string BuildCardsLeftOfThisSpecialString()
        {
            int numCardsToLeft = GetNumberOfCardsToTheLeftOfThisOne(base.Card).Value;
            string cardsToLeftString = "There ";
            if (numCardsToLeft == 1)
            {
                cardsToLeftString += "is 1 card ";
            }
            else if (numCardsToLeft == 0)
            {
                cardsToLeftString += "are no cards ";
            }
            else
            {
                cardsToLeftString += "are " + numCardsToLeft + " cards ";
            }

            cardsToLeftString += "to the left of this one.";

            return cardsToLeftString;
        }

        protected string BuildCardsRightOfThisSpecialString()
        {
            int numCardsToRight = GetNumberOfCardsToTheRightOfThisOne(base.Card).Value;
            string cardsToRightString = "There ";
            if (numCardsToRight == 1)
            {
                cardsToRightString += "is 1 card ";
            }
            else if (numCardsToRight == 0)
            {
                cardsToRightString += "are no cards ";
            }
            else
            {
                cardsToRightString += "are " + numCardsToRight + " cards ";
            }

            cardsToRightString += "to the right of this one.";

            return cardsToRightString;
        }
    }
}