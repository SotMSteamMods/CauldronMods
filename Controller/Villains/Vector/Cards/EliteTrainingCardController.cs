using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class EliteTrainingCardController : CardController
    {
        //==============================================================
        // Increase damage dealt by villain targets by 1.
        // When this card is destroyed, each player must discard 2 cards.
        //==============================================================

        public static readonly string Identifier = "EliteTraining";

        private const int IncreaseDamageAmount = 1;
        private const int CardsToDiscard = 2;

        public EliteTrainingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddWhenDestroyedTrigger(DestroyCardResponse, TriggerType.DiscardCard);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(IncreaseDamageAmount);
            idse.UntilCardLeavesPlay(this.Card);
            idse.SourceCriteria.IsVillain = true;
            idse.SourceCriteria.IsTarget = true;

            IEnumerator routine = base.AddStatusEffect(idse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            IEnumerator routine = base.GameController.EachPlayerDiscardsCards(CardsToDiscard, CardsToDiscard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}