using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.StSimeonsCatacombs
{
    public abstract class StSimeonsRoomCardController : StSimeonsBaseCardController
    {

        protected StSimeonsRoomCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            if (rooms.Contains(card) && cardSource.Card.Identifier != StSimeonsCatacombsInstructionsCardController.Identifier)
            {
                return false;
            }
            return true;
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            if (!base.Card.IsInPlayAndHasGameText)
            {
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => c.Identifier != StSimeonsCatacombsInstructionsCardController.Identifier, (Card c) => c == base.Card);

                if (flag != null && flag.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<Card> FindAllRoomsButThisOne()
        {
            //As of Sentinels 4.1.1, CardController.FindCardsWhere automatically checks card visibility, which caused an infinite loop since AskIfCardIsVisibleToCardSource references FindAllRoomsButThisOne.
            //Fixed by using GameController.FindCardsWhere instead, and making sure the parameter visibleToCard is null
            return GameController.FindCardsWhere((Card c) => IsDefinitionRoom(c) && c != Card, false, null, BattleZone);
        }
    }
}
