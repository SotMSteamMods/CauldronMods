using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class DelayedSymptomsCardController : CardController
    {
        //==============================================================
        // Destroy {H - 1} hero ongoing and/or equipment cards.
        // Discard the top card of each hero deck.
        //==============================================================

        public static readonly string Identifier = "DelayedSymptoms";

        private const int CardsToDiscard = 1;

        public DelayedSymptomsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            int cardsToDestroy = base.Game.H - 1;

            IEnumerator destroyRoutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker,
                new LinqCardCriteria(c => (IsOngoing(c) || IsEquipment(c)) && IsHero(c.Owner)), cardsToDestroy,
                cardSource: GetCardSource());

            IEnumerator discardRoutine = this.GameController.DiscardTopCardsOfDecks(this.DecisionMaker, l => l.IsHero, 
                CardsToDiscard, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
                yield return base.GameController.StartCoroutine(discardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
                base.GameController.ExhaustCoroutine(discardRoutine);
            }
        }
    }
}