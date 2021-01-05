using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsInstructionsCardController : VillainCharacterCardController
    {
        public static readonly string Identifier = "StSimeonsCatacombsInstructions";

        public StSimeonsCatacombsInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildNumberOfRoomsEnteredPlaySpecialString()).Condition = () => Card.IsFlipped;
            SpecialStringMaker.ShowSpecialString(() => "Environment cards cannot be played.", showInEffectsList: () => true).Condition = () => !Card.IsFlipped;
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            CannotPlayCards(ttc => ttc == FindTurnTakerController(TurnTaker) && !Card.IsFlipped);
            //At the end of the environment turn, put a random room card from beneath this one into play and flip this card
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.MoveRandomRoomIntoPlayThenFlip, TriggerType.MoveCard, additionalCriteria: pca => !Card.IsFlipped);
            //Whenever a room card would leave play, instead place it face up beneath this card. Then choose a different room beneath this card and put it into play.
            base.AddTrigger<MoveCardAction>((MoveCardAction mc) => Card.IsFlipped && mc.CardToMove.IsRoom && mc.Destination != base.TurnTaker.PlayArea && mc.Destination != Catacombs.UnderLocation, this.ChangeRoomPostLeaveResponse, TriggerType.MoveCard, TriggerTiming.Before);
            //If you change rooms this way three times in a turn, room cards become indestructible until the end of the turn.
            base.AddTrigger<GameAction>((GameAction ga) => Card.IsFlipped && NumberOfRoomsEnteredPlayThisTurn() >= 3 && !base.IsPropertyTrue("Indestructible"), SetRoomsIndestructibleResponse, TriggerType.CreateStatusEffect, TriggerTiming.Before);
            //At the end of the environment turn, if no room cards have entered play this turn, you may destroy a room card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.FreeDestroyRoomResponse, TriggerType.DestroyCard, (PhaseChangeAction pca) => Card.IsFlipped && NumberOfRoomsEnteredPlayThisTurn() == 0);
        }


        private IEnumerator MoveRandomRoomIntoPlayThenFlip(PhaseChangeAction pca)
        {
            string message = base.Card.Title + " puts a random room card from beneath this one into play";

            IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //puts a random room card from beneath this one into play
            IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, Catacombs.UnderLocation, false, true, false, new LinqCardCriteria((Card c) => this.IsDefinitionRoom(c), "room"), new int?(1), shuffleBeforehand: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            //flips this card
            IEnumerator coroutine3 = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }

            yield break;
        }

        private IEnumerator ChangeRoomPostLeaveResponse(MoveCardAction mc)
        {
            //Whenever a room card would leave play, instead place it face up beneath this card. 
            //Then choose a different room beneath this card and put it into play.
            IEnumerator cancel = base.CancelAction(mc);
            IEnumerator under = base.GameController.MoveCard(base.TurnTakerController, mc.CardToMove, Catacombs.UnderLocation, cardSource: base.GetCardSource());
            IEnumerator shuffle = base.GameController.ShuffleLocation(Catacombs.UnderLocation, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cancel);
                yield return base.GameController.StartCoroutine(under);
                yield return base.GameController.StartCoroutine(shuffle);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cancel);
                base.GameController.ExhaustCoroutine(under);
                base.GameController.ExhaustCoroutine(shuffle);
            }

            //only do immediate play action if its not living geometry, which will take care of room response
            if (mc.ActionSource == null || mc.ActionSource.CardSource == null || mc.ActionSource.CardSource.Card.Identifier != LivingGeometryCardController.Identifier)
            {
                //Then choose a different room beneath this card and put it into play.
                IEnumerator play = base.GameController.SelectAndPlayCard(this.DecisionMaker, Catacombs.UnderLocation.Cards.Where(c => c != mc.CardToMove), isPutIntoPlay: true);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(play);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(play);
                }
            }
            yield break;
        }

        private IEnumerator SetRoomsIndestructibleResponse(GameAction ga)
        {
            //room cards become indestructible until the end of the turn
            base.SetCardPropertyToTrueIfRealAction("Indestructible");
            MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
            makeIndestructibleStatusEffect.CardsToMakeIndestructible.HasAnyOfTheseKeywords = new List<string>() { "room" };
            makeIndestructibleStatusEffect.ToTurnPhaseExpiryCriteria.Phase = Phase.End;
            IEnumerator coroutine = base.AddStatusEffect(makeIndestructibleStatusEffect, true);

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

        private IEnumerator FreeDestroyRoomResponse(PhaseChangeAction pca)
        {
            if (IsRealAction())
            {
                Journal.RecordCardProperties(Card, "Indestructible", (bool?)null);
            }

            //you may destroy a room card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsRoom, "room"), true, cardSource: base.GetCardSource());
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


        private int NumberOfRoomsEnteredPlayThisTurn()
        {
            int result = (from e in base.GameController.Game.Journal.CardEntersPlayEntriesThisTurn()
                          where this.IsDefinitionRoom(e.Card)
                          select e).Count();

            return result;
        }

        private bool IsDefinitionRoom(Card card)
        {
            return card != null && card.Definition.Keywords.Contains("room");
        }

        private Card Catacombs
        {
            get
            {
                return base.FindCard(StSimeonsCatacombsCardController.Identifier);
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }

        private string BuildNumberOfRoomsEnteredPlaySpecialString()
        {
            int numRooms = NumberOfRoomsEnteredPlayThisTurn();
            string numRoomsString = "";
            if (numRooms == 0)
            {
                numRoomsString += "No rooms have";
            } else if(numRooms == 1)
            {
                numRoomsString += "1 room has";
            }
            else
            {
                numRoomsString += numRooms + " rooms have";
            }
            numRoomsString += " entered play this turn.";
            return numRoomsString;
        }
    }
}