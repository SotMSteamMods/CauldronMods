using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class TiamatSubCharacterCardController : VillainCharacterCardController
    {
        protected TiamatSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            AddTrigger((GameAction ga) => TurnTakerController is TiamatTurnTakerController tttc && !tttc.AreStartingCardsSetUp, (TurnTakerController as TiamatTurnTakerController).MoveStartingCards, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public bool IsSpell(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "spell");
        }

        public bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head");
        }

        protected bool DidDealDamageThisTurn(Card overrideCard = null)
        {
            //Did Tiamat Deal Damage This Turn
            return GameController.Game.Journal.DealDamageEntriesThisTurn().Any(e => e.SourceCard == (overrideCard ?? Card) && e.Amount > 0);
        }

        protected int GetNumberOfSpecificCardInTrash(string identifier)
        {
            return TurnTaker.Trash.Cards.Count(c => c.Identifier == identifier);
        }


        public override void AddTriggers()
        {
            base.AddTriggers();
            //Element of Lightning Cancel Draws
            base.CannotDrawCards(ElementOfLightningCriteria);
            //Clear Card Property preventing Draw
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.ClearLightningEffectResponse, TriggerType.Hidden);
        }

        protected bool ElementOfLightningCriteria(TurnTakerController ttc)
        {
            if (ttc is HeroTurnTakerController httc)
            {
                bool? criteriaMet =  Game.Journal.GetCardPropertiesStringList(Card, ElementOfLightningCardController.PreventDrawPropertyKey)?.Any(id => id == httc.TurnTaker.Identifier);
                return criteriaMet is null ? false : criteriaMet.Value;
            }
            return false;
        }

        protected IEnumerator ClearLightningEffectResponse(PhaseChangeAction action)
        {
            //Clear the secret property from this Character Card
            List<string> empty = new List<string>();
            GameController.AddCardPropertyJournalEntry(Card, ElementOfLightningCardController.PreventDrawPropertyKey, empty);
            yield break;
        }
    }
}