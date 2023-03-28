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
            ShowSpecialStringAugmentsInPlay();
        }

        public override void AddTriggers()
        {
            // When a non-hero card enters play, you may destroy this card.
            base.AddTrigger<CardEntersPlayAction>(p => !IsHero(p.CardEnteringPlay) && GameController.IsCardVisibleToCardSource(p.CardEnteringPlay, GetCardSource()),
                DestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After);

            base.AddTriggers();
        }

        private IEnumerator DestroySelfResponse(CardEntersPlayAction _)
        {

            IEnumerator routine;
            if (!base.GameController.IsCardIndestructible(base.Card))
            {
                // Ask player if they want to destroy this card
                List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
                routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                    SelectionType.DestroyCard, base.Card, storedResults: storedResults, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                if (!base.DidPlayerAnswerYes(storedResults))
                {
                    yield break;
                }

                //"May destroy this... If you do"
                //based on Bunker's Auxiliary Power Source, which uses the same wording
                AddAfterDestroyedAction(MoveAugmentsAndHeal);

                // Destroy this card
                List<DestroyCardAction> dcas = new List<DestroyCardAction>();
                routine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, storedResults: dcas, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                //if the destruction manages to get cancelled, clean up the trigger
                RemoveAfterDestroyedAction(MoveAugmentsAndHeal);
            }
            else
            {
                routine = base.GameController.SendMessageAction(base.Card.Title + " is indestructible, so it cannot be destroyed for extra effects.", 
                    Priority.Medium, GetCardSource(), null, true);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            yield break;
        }

        private IEnumerator MoveAugmentsAndHeal(GameAction _)
        {
            IEnumerator routine;

            // If you do, select any number of Augments in play and move each one next to a new hero. 

            var augmentsToMove = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => IsInPlayAugment(c), SelectionType.Custom,  null, false, 0, eliminateOptions: true, cardSource: GetCardSource());
            routine = GameController.SelectCardsAndDoAction(augmentsToMove, MoveInPlayAugment, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // ...Then, each augmented hero regains 2HP.
            routine = this.GameController.GainHP(this.HeroTurnTakerController, IsAugmentedHeroCharacterCard, HpGain,
                cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select Augments in play to move next to new heroes.", "Selecting Augments in play to move next to new heroes.", "Vote for Augments in play to move next to new heroes.", "move Augments in play next to new heroes");

        }
    }
}