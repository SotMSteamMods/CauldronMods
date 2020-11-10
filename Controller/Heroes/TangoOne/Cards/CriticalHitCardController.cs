using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            base.AddTrigger<DealDamageAction>(dda => dda.DamageSource.IsHero,
                new Func<DealDamageAction, IEnumerator>(this.RevealTopCardFromDeckResponse),
                new TriggerType[]
                {
                    TriggerType.DealDamage

                }, TriggerTiming.Before, null, false, true, true);

            base.AddTriggers();
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to discard off the top of their deck
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.DiscardFromDeck, this.Card, null, storedYesNoResults, null, GetCardSource(null));

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

            // Check to see if the card was moved and contains the keyword "critical", if it didn't, damage proceeds
            if (moveCardActions.Count <= 0 || !moveCardActions.First().WasCardMoved ||
                !IsCritical(moveCardActions.First().CardToMove))
            {
                yield break;
            }

            // Card had the "critical" keyword, cancel the damage
            //IEnumerator cancelDamageRoutine = base.CancelAction(dda);
            IEnumerator cancelDamageRoutine = base.GameController.SelectHeroAndIncreaseNextDamageDealt(this.HeroTurnTakerController, DamageIncrease, 1);
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
