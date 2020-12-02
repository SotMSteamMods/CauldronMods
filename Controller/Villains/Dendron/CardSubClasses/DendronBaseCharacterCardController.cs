using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public abstract class DendronBaseCharacterCardController : VillainCharacterCardController
    {
        //This card hosts the work-around logic for Choking Inscription

        protected DendronBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private bool ChokingInscriptionCriteria(TurnTakerController ttc)
        {
            if (ttc is HeroTurnTakerController httc)
            {
                return httc.CharacterCardControllers.Any(chc => chc.GetCardPropertyJournalEntryBoolean(ChokingInscriptionCardController.PreventDrawPropertyKey) == true);
            }
            return false;
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            base.CannotDrawCards(ChokingInscriptionCriteria);
        }

        protected bool IsTattoo(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tattoo");
        }
    }
}