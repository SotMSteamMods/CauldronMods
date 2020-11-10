using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.DocHavoc
{
    public class ImprovisedMineCardController : CardController
    {
        //==============================================================
        // When this card enters play, discard an Equipment card or this card is destroyed.
        // When a villain card would enter play, before it is revealed you may destroy this card.
        // If you do, prevent that card from entering play, and {DocHavoc} deals 1 target 2 fire damage.
        //==============================================================

        public static string Identifier = "ImprovisedMine";

        private const int DamageAmount = 2;

        public ImprovisedMineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction c) => c.CardEnteringPlay != base.Card 
                && base.IsVillain(c.CardEnteringPlay) && !c.Origin.IsInPlay 
                && base.GetCardPropertyJournalEntryCard("CardBlocked") == null 
                && base.GameController.IsCardVisibleToCardSource(c.CardEnteringPlay, base.GetCardSource(null)), 
                new Func<CardEntersPlayAction, IEnumerator>(this.DiscardAndDestroyResponse), new TriggerType[]
            {
                TriggerType.CancelAction
            }, TriggerTiming.Before, null, false, true, 
                null, false, null, null, false, false);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();

            IEnumerator discardCardRoutine = base.GameController.SelectAndDiscardCard(this.HeroTurnTakerController, true, card => IsEquipment(card),
                storedResults, cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            // If an equipment card wasn't discarded, destroy this card
            if (!DidDiscardCards(storedResults, 1))
            {
                IEnumerator destroyCardRoutine = base.GameController.DestroyCard(this.DecisionMaker, this.Card,
                    false, null, null, null, null, null,
                    null, null, null, base.GetCardSource(null));

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCardRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCardRoutine);
                }
            }
        }

        private IEnumerator DiscardAndDestroyResponse(CardEntersPlayAction action)
        {
            Card card = action.CardEnteringPlay;
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();

            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, 
                SelectionType.MoveCardOnDeck, card, null, storedResults, null, base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Return if they chose not to cancel the villain card from being played
            if (!base.DidPlayerAnswerYes(storedResults))
            {
                yield break;
            }

            base.AddCardPropertyJournalEntry("CardBlocked", card);

            // Cancel the villain card from being played
            routine = base.CancelAction(action, true, true, null, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            
            CardController cc = base.FindCardController(card);
            routine = base.GameController.MoveCard(base.TurnTakerController, card, cc.TurnTaker.Deck, 
                false, false, true, null, false, 
                null, base.TurnTaker, null, true, false, null, 
                true, false, false, 
                false, base.GetCardSource(null));
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
            

            // Destroy this card
            routine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (base.Card.IsInPlay)
            {
                Card card2 = null;
                base.AddCardPropertyJournalEntry("CardBlocked", card2);
            }
            yield break;
        }
    }
}
