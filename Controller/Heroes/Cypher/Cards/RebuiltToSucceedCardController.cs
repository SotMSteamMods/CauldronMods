using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class RebuiltToSucceedCardController : CypherBaseCardController
    {
        //==============================================================
        // Select two Augments in your trash. Put one into your hand and one into play.
        // The hero you augment this way may play a card now.
        //==============================================================

        public static string Identifier = "RebuiltToSucceed";

        public RebuiltToSucceedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {

            /*
            var scsd = new SelectCardsDecision(GameController, this.HeroTurnTakerController, c => c.Location == TurnTaker.Trash, SelectionType.MoveCard,
                numberOfCards: 2,
                isOptional: false,
                requiredDecisions: 0,
                allowAutoDecide: true,
                cardSource: GetCardSource());


            IEnumerator coroutine = GameController.SelectCardsAndDoAction(scsd, scd => base.GameController.Moveca)
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            */

            IEnumerable<MoveCardDestination> moveLocations = new[]
            {
                new MoveCardDestination(base.TurnTaker.PlayArea),
                new MoveCardDestination(base.HeroTurnTaker.Hand)
            };

            IEnumerator routine = base.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController,
                this.HeroTurnTaker.Trash, 0, 2, new LinqCardCriteria(IsAugment), moveLocations,
                cardSource: GetCardSource());


            //IEnumerator routine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, base.GetTitanform(), locations, true, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(routine);
            }
            else
            {
                GameController.ExhaustCoroutine(routine);
            }

        }
    }
}