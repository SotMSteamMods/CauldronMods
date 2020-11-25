using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.StSimeonsCatacombs
{
    public abstract class StSimeonsGhostCardController : StSimeonsBaseCardController
    {
        protected StSimeonsGhostCardController(Card card, TurnTakerController turnTakerController, string[] affectedIdentifiers, bool flipIdentiferInPlayCondition = false) : base(card, turnTakerController)
        {
            this.AffectedIdentifiers = affectedIdentifiers;
            this.FlipIdentiferInPlayCondition = flipIdentiferInPlayCondition;
            //TODO: Add a conditional special string that says something like "AffectedCard is in play so it is affected by Hero cards" and the reverse
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public override void AddTriggers()
        {
            //This card may not be affected by hero cards unless AffectedCard is in play.
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => !this.IsAffectedCardInPlay() && md.CardSource != null && md.CardSource.Card.Owner.IsHero, this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
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
            if (!this.IsAffectedCardInPlay() && card == base.Card && cardSource.Card.IsHero)
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
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => c.IsHero, (Card c) => c == base.Card);

                if (flag != null && flag.Value)
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsAffectedCardInPlay()
        {
            bool identiferInPlay = base.FindCardsWhere(c => c.IsInPlayAndHasGameText && AffectedIdentifiers.Contains(c.Identifier)).Any();
            return FlipIdentiferInPlayCondition ? !identiferInPlay : identiferInPlay;
        }

        protected string[] AffectedIdentifiers { get; }
        protected bool FlipIdentiferInPlayCondition { get; }
    }
}