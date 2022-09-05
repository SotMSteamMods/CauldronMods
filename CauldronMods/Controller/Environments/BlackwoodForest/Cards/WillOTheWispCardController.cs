using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class WillOTheWispCardController : CardController
    {
        //==============================================================
        // Environment cards may not be played during the environment turn.
        // At the end of the villain turn play the top 2 cards of the environment deck. 
        //==============================================================

        public static readonly string Identifier = "WillOTheWisp";

        private const int EnvironmentCardsToDraw = 2;

        public WillOTheWispCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Environment cards may not be played during the environment turn
            base.CannotPlayCards(cardCriteria: (Card c) =>  base.Game.ActiveTurnPhase.IsEnvironment && c.Owner.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
            // At the end of the villain turn play the top 2 cards of the environment deck.
            base.AddEndOfTurnTrigger(tt => tt.IsVillain && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), PlayCardsResponse, TriggerType.PlayCard);

            base.AddTriggers();
        }

        private IEnumerator PlayCardsResponse(PhaseChangeAction pca)
        {
            var coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, false, EnvironmentCardsToDraw, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}