using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.StSimeonsCatacombs
{
    public class RoomCardController : CardController
    {

        public RoomCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        
        public override void AddTriggers()
        {
            //No other rooms may be affected besides the one in play
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.Decisions.Any((IDecision d) => d.SelectedCard.IsRoom) && md.CardSource != null && md.CardSource.Card.Identifier != "StSimeonsCatacombsInstructions", this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
        }


        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove all other roomms as options
            IEnumerable<Card> rooms = FindAllRoomsButThisOne();
            md.RemoveDecisions((IDecision d) => rooms.Contains(d.SelectedCard));
            yield return base.DoNothing();
            yield break;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            IEnumerable<Card> rooms = FindAllRoomsButThisOne();
            if (rooms.Contains(card) && cardSource.Card.Identifier != "StSimeonsCatacombsInstructions")
            {
                return new bool?(false);
            }
            return new bool?(true);
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            if (!base.Card.IsInPlayAndHasGameText)
            {
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => c.Identifier != "StSimeonsCatacombsInstructions", (Card c) => c == base.Card);

                if (flag != null && flag.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsDefinitionRoom(Card card)
        {
            return card != null && card.Definition.Keywords.Contains("room");
        }

        private IEnumerable<Card> FindAllRoomsButThisOne()
        {
            return FindCardsWhere((Card c) => IsDefinitionRoom(c) && c != base.Card);
        }

    }
}