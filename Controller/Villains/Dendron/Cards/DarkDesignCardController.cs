using System.Collections;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class DarkDesignCardController : CardController
    {
        //==============================================================
        // The last hero that dealt damage to a villain target
        // must discard {H} cards from their hand.
        //==============================================================

        public static readonly string Identifier = "DarkDesign";

        public DarkDesignCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            Card lastHeroToDamageVillainTarget = GetLastHeroToDamageVillainTarget();
            if (lastHeroToDamageVillainTarget != null)
            {
                TurnTakerController ttc = this.GameController.FindTurnTakerController(lastHeroToDamageVillainTarget.Owner);

                // Discard {H} cards from hand
                int cardsToDiscard = Game.H;
                IEnumerator discardCardsRoutine = this.GameController.SelectAndDiscardCards(ttc.ToHero(), cardsToDiscard, false, cardsToDiscard);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCardsRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCardsRoutine);
                }
            }
        }

        private Card GetLastHeroToDamageVillainTarget()
        {
            DealDamageJournalEntry journalEntry = base.GameController.Game.Journal.DealDamageEntries().LastOrDefault(j => j.TargetCard.IsVillainTarget && j.CardThatCausedDamageToOccur.IsHero);

            return journalEntry?.CardThatCausedDamageToOccur;
        }
    }
}