﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron
{
    public class GhostCardController : CardController
    {
        #region Constructors

        public GhostCardController(Card card, TurnTakerController turnTakerController, string[] affectedIdentifiers, bool reversed) : base(card, turnTakerController)
        {
            this.AffectedIdentifiers = affectedIdentifiers;
            this.Reversed = reversed;
            //TODO: Add a conditional special string that says something like "AffectedCard is in play so it is affected by Hero cards" and the reverse
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //This card may not be affected by hero cards unless AffectedCard is in play.
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => !this.IsAffectedCardInPlay() && md.CardSource != null && md.CardSource.Card.Owner.IsHero, new Func<MakeDecisionsAction, IEnumerator>(this.RemoveDecisionsFromMakeDecisionsResponse), TriggerType.RemoveDecision, TriggerTiming.Before);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => d.SelectedCard == base.Card);
            yield return base.DoNothing();
            yield break;
        }

        private bool FlipIfReversed(bool value)
        {
            if(Reversed)
            {
                return !value;
            }

            return value;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            //check if card is from a hero and if AffectedCard is in play
            if (!this.IsAffectedCardInPlay() && card == base.Card && cardSource.Card.IsHero)
            {
                return new bool?(false);
            }
            return new bool?(true);
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
            return FlipIfReversed(base.FindCardsWhere(c => c.IsInPlayAndHasGameText && AffectedIdentifiers.Contains(c.Identifier)).Count() > 0);
        }

        public string[] AffectedIdentifiers { get; private set; }
        public bool Reversed { get; private set; }
        #endregion Methods
    }
}