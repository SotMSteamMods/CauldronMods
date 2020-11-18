using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class ChameleonArmorCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Whenever {TangoOne} would be dealt damage, you may discard
        // the top card of your deck. If it is a Critical card, prevent that damage.
        //==============================================================

        public static string Identifier = "ChameleonArmor";

        public ChameleonArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>(dda => dda.Target.Equals(this.CharacterCard),
                this.RevealTopCardFromDeckResponse,
                new TriggerType[]
                {

                }, TriggerTiming.Before, null, false, true, true);

            base.AddTriggers();
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to discard off the top of their deck
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.DiscardFromDeck, this.Card, null, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Return if they chose not to discard from their deck
            if (!base.DidPlayerAnswerYes(storedYesNoResults))
            {
                yield break;
            }

            // Move card from top of their deck to the trash
            List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
            IEnumerator discardCardRoutine 
                = base.GameController.DiscardTopCard(this.TurnTaker.Deck, moveCardActions, null, this.TurnTaker, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            // Check to see if the card was moved and contains the keyword "critical", if it didn't, damage proceeds
            if (moveCardActions.Count <= 0 || !moveCardActions.First().WasCardMoved ||
                !IsCritical(moveCardActions.First().CardToMove) )
            {
                yield break;
            }

            // Card had the "critical" keyword, cancel the damage
            IEnumerator cancelDamageRoutine = base.CancelAction(dda);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cancelDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cancelDamageRoutine);
            }
        }
    }
}
