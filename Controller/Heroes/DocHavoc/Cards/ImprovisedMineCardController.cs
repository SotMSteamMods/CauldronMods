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
           
            base.AddTrigger<PlayCardAction>((PlayCardAction pc) => base.IsVillain(pc.CardToPlay) && !pc.IsPutIntoPlay, new Func<PlayCardAction, IEnumerator>(this.DiscardAndDestroyCardResponse), new TriggerType[]
            {
                TriggerType.DestroySelf
            }, TriggerTiming.Before);

            base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.WhenDestroyedResponse), new TriggerType[]
                {
                    TriggerType.CancelAction,
                    TriggerType.DealDamage
                });
        }

        private IEnumerator WhenDestroyedResponse(DestroyCardAction dca)
        {
            PlayCardAction action = this._playCardToCancel;
            this._playCardToCancel = null;
            if (action == null)
            {
                yield break;
            }
            Card card = action.CardToPlay;
            IEnumerator coroutine = base.CancelAction(action, isPreventEffect: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            

            // Deal 1 target 2 Fire damage
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(this.GameController, this.CharacterCard), DamageAmount, DamageType.Fire,
                new int?(1), false,
                new int?(1), cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private IEnumerator DiscardAndDestroyCardResponse(PlayCardAction pca)
        {
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();

            //ask player if they want to destroy this card
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.DestroySelf, base.Card, storedResults: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Return if they chose to not cancel the destruction
            if (!base.DidPlayerAnswerYes(storedResults))
            {
                yield break;
            }

            //set the playCardToCancel
            this._playCardToCancel = pca;

            // Destroy this card
            coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }


        public override IEnumerator Play()
        {
            //equipment must be discarded upon entering play
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCardRoutine = base.GameController.SelectAndDiscardCard(this.HeroTurnTakerController, additionalCriteria: card => IsEquipment(card),
                storedResults: storedResults, cardSource: this.GetCardSource());

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

        //local variable to store the PlayCardAction in
        private PlayCardAction _playCardToCancel = null;
   
    }
}
