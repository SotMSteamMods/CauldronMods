using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class HydraTiamatCharacterCardController : TiamatSubCharacterCardController
    {
        protected HydraTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //challenge special string
            //the actual rule lives on Thunderous Gale instructions
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsHead(c) && c.IsInPlayAndHasGameText && !c.IsFlipped, "non-decapitated head")).Condition = () => Game.IsChallenge;
        }

        //Face down heads are indestructible
        public override bool CanBeDestroyed
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeMovedOutOfGame => false;
        protected abstract ITrigger[] AddFrontTriggers();

        public override void AddTriggers()
        {
            // After any action, if there are fewer than 6 heads in the game, game over.
            AddTrigger((GameAction a) => !(a is MessageAction) && !base.TurnTakerController.IsGameWinnable(), UnwinnableGameOver, TriggerType.Hidden, TriggerTiming.After);
        }

        private IEnumerator UnwinnableGameOver(GameAction a)
        {
            IEnumerator coroutine = base.GameController.SendMessageAction("Victory is now impossible for the heroes.", Priority.Critical, GetCardSource());
            IEnumerator e = base.GameController.GameOver(EndingResult.AlternateDefeat, "The heroes are defeated!", showEndingTextAsMessage: false, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(e);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(e);
            }
        }

        public override void AddSideTriggers()
        {
            //Win Condition handled on Noxious Fire Instructions

            //Front Triggers
            if (!base.Card.IsFlipped)
            {
                base.AddSideTriggers(this.AddFrontTriggers());
            }
            else
            {
                //Decapitated heads cannot deal damage
                base.AddSideTrigger(base.AddPreventDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.Card));
            }
        }

        //When a {Tiamat} head is destroyed, flip her.
        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            FlipCardAction action = new FlipCardAction(base.GameController, this, false, false, destroyCard.ActionSource);
            IEnumerator coroutine = base.DoAction(action);
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

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            CardSource cardSource = flip.CardSource;
            if (cardSource == null && flip.ActionSource != null)
            {
                cardSource = flip.ActionSource.CardSource;
            }
            if (cardSource == null)
            {
                cardSource = base.GetCardSource();
            }

            if (!flip.CardToFlip.Card.IsFlipped)
            {
                IEnumerator coroutine = base.GameController.RemoveTarget(base.Card, cardSource: cardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
