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

        public static readonly string Identifier = "CriticalHit";

        private const int DamageIncrease = 3;

        public CriticalHitCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(this.HeroTurnTaker.Deck, new LinqCardCriteria(c => IsCritical(c), "critical"));

            this.AllowFastCoroutinesDuringPretend = false;
            this.RunModifyDamageAmountSimulationForThisCard = true;
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>(dda => dda.DamageSource != null && dda.DamageSource.IsSameCard(base.CharacterCard) && !dda.IsPretend,
                this.RevealTopCardFromDeckResponse,
                new TriggerType[] { TriggerType.IncreaseDamage },
                TriggerTiming.Before, null, isConditional: false, requireActionSuccess: true, isActionOptional: true);
            // Add an IncreaseDamageTrigger in Pretend to get a better damage preview (but it still doesn't seem to work as intended?)
            AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.IsSameCard(base.CharacterCard) && dda.IsPretend, dda => 3);
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to discard off the top of their deck
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.RevealTopCardOfDeck, this.Card, dda, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Return if they chose not to discard from their deck
            if (!base.DidPlayerAnswerYes(storedYesNoResults))
            {
                yield break;
            }

            // Move card from top of their deck to the trash
            List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
            coroutine = base.GameController.DiscardTopCard(this.TurnTaker.Deck, moveCardActions, card => true, this.TurnTaker, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidMoveCard(moveCardActions))
            {
                Card discardedCard = moveCardActions.First().CardToMove;
                if (IsCritical(discardedCard))
                {
                    // Card had the "critical" keyword, increase the damage

                    coroutine = base.GameController.SendMessageAction(discardedCard.Title + " is a critical card!", Priority.Medium, base.GetCardSource(), associatedCards: new Card[] { discardedCard });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = GameController.IncreaseDamage(dda, DamageIncrease, false, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }
    }
}
