using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class CriticalHitCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Whenever {TangoOne} deals damage, you may discard the top card of your deck.
        // If it is a Critical card, increase that damage by 3.
        //==============================================================

        public static string Identifier = "CriticalHit";

        private const int DamageIncrease = 3;

        public CriticalHitCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            DamageSource heroDamageSource = new DamageSource(this.GameController, this.Card.Owner.CharacterCard);

            base.AddTrigger<DealDamageAction>(dda => dda.DamageSource != null && dda.DamageSource.IsSameCard(base.CharacterCard),
                this.RevealTopCardFromDeckResponse,
                new TriggerType[]
                {
                    TriggerType.DealDamage

                }, TriggerTiming.Before, null, false, true, true);

            base.AddTriggers();
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            // Ask if player wants to discard off the top of their deck

            YesNoDecision yesNo = new YesNoDecision(base.GameController, base.HeroTurnTakerController,
                SelectionType.DiscardFromDeck, false, cardSource: GetCardSource());
            
            IEnumerator routine = base.GameController.MakeDecisionAction(yesNo, true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Return if they chose not to discard from their deck
            if (yesNo.Answer == null || !yesNo.Answer.Value)
            {
                yield break;
            }

            // Move card from top of their deck to the trash
            List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
            IEnumerator discardCardRoutine = base.GameController.DiscardTopCard(this.TurnTaker.Deck, moveCardActions,
                null, this.TurnTaker, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            // Check to see if the card was moved and contains the keyword "critical", if it didn't, break and damage proceeds normally
            if (moveCardActions.Count <= 0 || !moveCardActions.First().WasCardMoved ||
                !IsCritical(moveCardActions.First().CardToMove))
            {
                yield break;
            }

            // Card had the "critical" keyword, increase the damage
            ModifyDealDamageAction mdda = new IncreaseDamageAction(this.GameController, dda, DamageIncrease, false);
            dda.AddDamageModifier(mdda);

        }
    }
}
