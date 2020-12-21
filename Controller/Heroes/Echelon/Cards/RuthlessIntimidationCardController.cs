using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class RuthlessIntimidationCardController : CardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.
        // Increase damage dealt to the non-hero target with the lowest HP by 1.
        //==============================================================

        public static string Identifier = "RuthlessIntimidation";

        public RuthlessIntimidationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(ttc => ttc == this.TurnTaker, StartOfTurnResponse, TriggerType.DiscardCard);

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {

            List<DiscardCardAction> results = new List<DiscardCardAction>();
            IEnumerator routine = GameController.SelectAndDiscardCard(DecisionMaker, optional: true, storedResults: results, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // .. If you do not, draw a card and destroy this card.
            if (!DidDiscardCards(results, 1))
            {
                routine = this.DrawCard(this.HeroTurnTaker);
                IEnumerator routine2 = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                    yield return base.GameController.StartCoroutine(routine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                    base.GameController.ExhaustCoroutine(routine2);
                }
            }
        }
    }
}