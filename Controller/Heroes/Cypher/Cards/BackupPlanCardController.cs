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

            IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
            {
                //new Function(base.FindCardController(c).DecisionMaker, "Deal self 3 toxic damage to play a card now.", SelectionType.PlayCard, () => this.DealDamageAndDrawResponse(c))
                //new Function(this.HeroTurnTakerController, "Move augment", SelectionType.MoveCardNextToCard, )
            };


            //base.GameController.SelectCardsAndPerformFunction(this.HeroTurnTakerController, new LinqCardCriteria(c => GetAugmentsInPlay().Contains(c)), ), 

            // Then, each augmented hero regains 2HP.
            routine = this.GameController.GainHP(this.HeroTurnTakerController, IsAugmented, HpGain,
                cardSource: GetCardSource());

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