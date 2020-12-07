using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class BackupPlanCardController : CypherBaseCardController
    {
        //==============================================================
        // When a non-hero card enters play, you may destroy this card.
        // If you do, select any number of Augments in play and move
        // each one next to a new hero. Then, each augmented hero regains 2HP.
        //==============================================================

        public static string Identifier = "BackupPlan";

        private const int HpGain = 2;


        public BackupPlanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // When a non-hero card enters play, you may destroy this card.
            base.AddTrigger<CardEntersPlayAction>(p => !p.CardEnteringPlay.IsHero && GameController.IsCardVisibleToCardSource(p.CardEnteringPlay, GetCardSource()),
                DestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After);

            base.AddTriggers();
        }

        private IEnumerator DestroySelfResponse(CardEntersPlayAction cepa)
        {
            List<DestroyCardAction> dcas = new List<DestroyCardAction>();
            IEnumerator routine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, true, dcas, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!base.DidDestroyCard(dcas))
            {
                yield break;
            }

            // If you do, select any number of Augments in play and move
            // each one next to a new hero. Then, each augmented hero regains 2HP.
            List<Card> augmentsInPlay = GetAugmentsInPlay();
            //base.GameController.SelectCardsAndPerformFunction(this.HeroTurnTakerController)


        }
    }
}