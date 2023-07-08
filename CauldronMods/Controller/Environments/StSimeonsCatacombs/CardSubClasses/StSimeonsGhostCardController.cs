using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.StSimeonsCatacombs
{
    public abstract class StSimeonsGhostCardController : StSimeonsBaseCardController
    {
        protected StSimeonsGhostCardController(Card card, TurnTakerController turnTakerController, string[] affectedIdentifiers, bool flipIdentiferInPlayCondition = false) : base(card, turnTakerController)
        {
            this.AffectedIdentifiers = affectedIdentifiers;
            this.FlipIdentiferInPlayCondition = flipIdentiferInPlayCondition;
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            SpecialStringMaker.ShowSpecialString(() => BuildAffectedCardString());
        }

        public override void AddTriggers()
        {
            //This card may not be affected by hero cards unless AffectedCard is in play.
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => !this.IsAffectedCardInPlay() && md.CardSource != null && IsHero(md.CardSource.Card), this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => d.SelectedCard == base.Card);
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            //check if card is from a hero and if AffectedCard is in play
            if (!this.IsAffectedCardInPlay() && card == base.Card && IsHero(cardSource.Card))
            {
                return false;
            }
            return true;
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            //if AffectedCard is not in play, actions from hero cards cannot affect this card
            if (!this.IsAffectedCardInPlay())
            {
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => IsHero(c), (Card c) => c == base.Card);

                if (flag != null && flag.Value)
                {
                    return false;
                }
            }
            return true;
        }
        protected bool IsAffectedCardInPlay()
        {
            bool identiferInPlay = base.FindCardsWhere(c => c.IsInPlayAndHasGameText && AffectedIdentifiers.Contains(c.Identifier)).Any();
            return FlipIdentiferInPlayCondition ? !identiferInPlay : identiferInPlay;
        }

        protected string[] AffectedIdentifiers { get; }
        protected bool FlipIdentiferInPlayCondition { get; }

        protected Card GetAffectedCardInPlay()
        {
            IEnumerable<Card> cardsInPlay = base.FindCardsWhere(c => (FlipIdentiferInPlayCondition ? !c.IsInPlayAndHasGameText : c.IsInPlayAndHasGameText) && AffectedIdentifiers.Contains(c.Identifier));
            return cardsInPlay.FirstOrDefault();
        }

        protected IEnumerable<Card> AffectedCards
        {
            get
            {
                return FindCardsWhere(c => AffectedIdentifiers.Contains(c.Identifier));
            }
        }

        private string BuildAffectedCardString()
        {
            var affectedRoomInPlay = GetAffectedCardInPlay(); ;
            string affectedRoomString = "";
            if (affectedRoomInPlay != null)
            {

                affectedRoomString += affectedRoomInPlay.Title + " is ";
                if (FlipIdentiferInPlayCondition)
                {
                    affectedRoomString += "not ";

                }
                affectedRoomString += "in play. " + Card.Title + " is affected by Hero cards.";
            }
            else if(AffectedCards.Count() > 0)
            {
                var affectedRoomsList = AffectedCards;
                affectedRoomString += affectedRoomsList.First().Title + " ";
                if (affectedRoomsList.Count() > 1)
                {
                    affectedRoomString += "and " + affectedRoomsList.Last().Title + " are ";
                }
                else
                {
                    affectedRoomString += "is ";
                }
                if (!FlipIdentiferInPlayCondition)
                {
                    affectedRoomString += "not ";
                }
                affectedRoomString += "in play. " + Card.Title + " is unaffected by Hero cards.";
            } else
            {
                string affectedWord = !FlipIdentiferInPlayCondition ? "unaffected" : "affected";
                affectedRoomString += "No rooms are visible to " + Card.Title + ". " + Card.Title + " is " + affectedWord + " by Hero cards.";
            }
            return affectedRoomString;
        }
    }
}