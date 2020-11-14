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

        public static string Identifier = "WillOTheWisp";

        private const int EnvironmentCardsToDraw = 2;

        public WillOTheWispCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the end of the villain turn play the top 2 cards of the environment deck.
            base.AddEndOfTurnTrigger(tt => tt.IsVillain, PlayCardsResponse, new[]
            {
                TriggerType.PlayCard
            });

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Environment cards may not be played during the environment turn
            PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect
            {
                ToTurnPhaseCriteria = {Phase = Phase.DrawCard, TurnTaker = this.TurnTaker}
            };

            IEnumerator preventDrawingRoutine = base.AddStatusEffect(ppase);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(preventDrawingRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(preventDrawingRoutine);
            }
        }

        private IEnumerator PlayCardsResponse(PhaseChangeAction pca)
        {
            IEnumerator drawCardsRoutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, 
                false, EnvironmentCardsToDraw);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardsRoutine);
            }
        }
    }
}