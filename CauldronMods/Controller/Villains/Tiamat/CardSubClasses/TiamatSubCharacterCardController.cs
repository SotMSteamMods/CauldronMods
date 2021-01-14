using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class TiamatSubCharacterCardController : VillainCharacterCardController
    {
        protected TiamatSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

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
                return httc.CharacterCardControllers.Any(chc => chc.GetCardPropertyJournalEntryBoolean(ElementOfLightningCardController.PreventDrawPropertyKey) == true);
            }
            return false;
        }

        protected IEnumerator ClearLightningEffectResponse(PhaseChangeAction action)
        {
            //Clear the secret property from all Character Cards 
            foreach (HeroTurnTaker hero in Game.HeroTurnTakers)
            {
                GameController.AddCardPropertyJournalEntry(hero.CharacterCard, ElementOfLightningCardController.PreventDrawPropertyKey, (bool?)null);
            }
            yield break;
        }
    }
}