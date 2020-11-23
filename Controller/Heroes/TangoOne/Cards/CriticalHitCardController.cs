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
            DamageSource heroDamageSource = new DamageSource(this.GameController, this.Card.Owner.CharacterCard);

            base.AddTrigger<DealDamageAction>(dda => dda.DamageSource != null && dda.DamageSource.IsSameCard(base.CharacterCard),
                this.RevealTopCardFromDeckResponse,
                new TriggerType[]
                {
                    TriggerType.IncreaseDamage

                }, TriggerTiming.Before, null, isConditional: false, requireActionSuccess: true, isActionOptional: true);

            base.AddTriggers();
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to discard off the top of their deck
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(DecisionMaker,
                SelectionType.DiscardFromDeck, this.Card, dda, storedYesNoResults, null, GetCardSource());

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
            IEnumerator discardCardRoutine = base.GameController.DiscardTopCard(this.TurnTaker.Deck, moveCardActions, card => true, this.TurnTaker, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            if (DidMoveCard(moveCardActions))
            {
                Card discardedCard = moveCardActions.First().CardToMove;
                if (IsCritical(discardedCard))
                {
                    // Card had the "critical" keyword, increase the damage
                    //ModifyDealDamageAction mdda = new IncreaseDamageAction(this.GameController, dda, DamageIncrease, false);
                    //dda.AddDamageModifier(mdda);

                    IEnumerator sendMessage = base.GameController.SendMessageAction(discardedCard.Title + " is a critical card, so damage is increased by 3!", Priority.Medium, base.GetCardSource(), associatedCards: new Card[] { discardedCard });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(sendMessage);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(sendMessage);
                    }

                    var coroutine = GameController.IncreaseDamage(dda, DamageIncrease, false, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(sendMessage);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(sendMessage);
                    }
                }
            }
            yield break;
        }
    }
}
