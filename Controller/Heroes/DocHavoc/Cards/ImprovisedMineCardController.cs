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

        public static readonly string Identifier = "ImprovisedMine";

        private const int DamageAmount = 2;

        public ImprovisedMineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            var types = new[] { TriggerType.DestroySelf, TriggerType.CancelAction, TriggerType.DealDamage };

            base.AddTrigger<PlayCardAction>((PlayCardAction pc) => base.IsVillain(pc.CardToPlay) && !pc.IsPutIntoPlay, this.DiscardAndDestroyCardResponse, types, TriggerTiming.Before, isActionOptional: true);
        }

        private IEnumerator WhenDestroyedResponse(PlayCardAction action)
        {
            IEnumerator coroutine = CancelAction(action, isPreventEffect: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Doc Havoc Deal 1 target 2 Fire damage, note CardSource must DH as this card is already out of play
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), DamageAmount, DamageType.Fire,
                        1, false, 1,
                        cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DiscardAndDestroyCardResponse(PlayCardAction pca)
        {
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();

            //ask player if they want to destroy this card
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroySelf, Card,
                                    storedResults: storedResults,
                                    cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Return if they chose to not cancel the destruction
            if (DidPlayerAnswerYes(storedResults))
            {
                // Destroy this card
                coroutine = GameController.DestroyCard(DecisionMaker, Card,
                            actionSource: pca,
                            postDestroyAction: () => WhenDestroyedResponse(pca),
                            cardSource: GetCardSource());
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

        public override IEnumerator Play()
        {
            //equipment must be discarded upon entering play
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCardRoutine = GameController.SelectAndDiscardCard(DecisionMaker,
                additionalCriteria: card => IsEquipment(card),
                storedResults: storedResults,
                cardSource: GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            // If an equipment card wasn't discarded, destroy this card
            if (!DidDiscardCards(storedResults, 1))
            {
                IEnumerator destroyCardRoutine = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());

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
    }
}
