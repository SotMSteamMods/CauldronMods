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

        public static readonly string Identifier = "ChameleonArmor";

        public ChameleonArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(this.HeroTurnTaker.Deck, new LinqCardCriteria(c => IsCritical(c), "critical"));

            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>(dda => dda.Target.Equals(this.CharacterCard) && dda.Amount > 0,
                this.RevealTopCardFromDeckResponse,
                new TriggerType[]
                {
            TriggerType.RevealCard,
            TriggerType.CancelAction
                }, TriggerTiming.Before
            );
        }

        private IEnumerator RevealTopCardFromDeckResponse(DealDamageAction dda)
        {
            if (GameController.PreviewMode)
            {
                var e = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.AmbiguousDecision, Card, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
                yield break;
            }

            if (dda.IsPretend) yield break;

            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
            // Ask if player wants to discard off the top of their deck
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(DecisionMaker,
                SelectionType.RevealTopCardOfDeck, this.Card, dda, storedYesNoResults, null, GetCardSource());

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
            //IEnumerator discardCardRoutine
            //    = base.GameController.DiscardTopCard(this.TurnTaker.Deck, moveCardActions, card => true, this.TurnTaker, base.GetCardSource());

            IEnumerator discardCardRoutine = GameController.DiscardTopCard(TurnTaker.Deck, moveCardActions, showCard:c => true, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            // Check to see if the card was moved and contains the keyword "critical", if it didn't, damage proceeds
            if (DidMoveCard(moveCardActions))
            {
                Card discardedCard = moveCardActions.First().CardToMove;
                if (IsCritical(discardedCard))
                {
                    IEnumerator sendMessage = base.GameController.SendMessageAction(discardedCard.Title + " is a critical card, so damage is prevented!", Priority.Medium, base.GetCardSource(), associatedCards: new Card[] { discardedCard });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(sendMessage);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(sendMessage);
                    }

                    // Card had the "critical" keyword, cancel the damage
                    IEnumerator cancelDamageRoutine = base.CancelAction(dda, isPreventEffect: true);
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

            yield break;
        }
    }
}
