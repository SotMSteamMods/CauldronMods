using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class KnowYourEnemyCardController : CardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card.
        // If you do not, draw a card and destroy this card.
        // The first time a hero destroys a non-hero target each turn, you may draw a card.
        //==============================================================

        public static string Identifier = "KnowYourEnemy";

        public KnowYourEnemyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == this.TurnTaker, StartOfTurnResponse,
                new[]
                {
                    TriggerType.DiscardCard, 
                    TriggerType.DrawCard, 
                    TriggerType.DestroySelf
                });

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            // you may discard a card.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator routine = base.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, true, 0, storedResults,
                cardCriteria: new LinqCardCriteria(c => true), cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (base.DidDiscardCards(storedResults, 1))
            {
                yield break;
            }

            // ... If you do not, draw a card and destroy this card.
            routine = base.DrawCard(this.HeroTurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
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