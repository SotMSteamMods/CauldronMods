using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public abstract class TiamatCharacterCardController : TiamatSubCharacterCardController
    {
        protected TiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override bool CanBeDestroyed
        {
            get
            {
                return false;
            }
        }

        protected abstract ITrigger[] AddFrontTriggers();
        protected abstract ITrigger[] AddFrontAdvancedTriggers();
        protected abstract ITrigger[] AddDecapitatedTriggers();
        protected abstract ITrigger[] AddDecapitatedAdvancedTriggers();

        public override void AddSideTriggers()
        {
            //Win Condition
            base.AddSideTrigger(base.AddTrigger<GameAction>(delegate (GameAction g)
            {
                if (base.GameController.HasGameStarted && !(g is GameOverAction) && !(g is IncrementAchievementAction))
                {
					return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsVillainTarget(c)).Count() == 0;
                }
                return false;
            }, (GameAction g) => base.DefeatedResponse(g), new TriggerType[]
            {
                TriggerType.GameOver,
                TriggerType.Hidden
            }, TriggerTiming.After));
            //Front Triggers
            if (!base.Card.IsFlipped)
            {
                base.AddSideTriggers(this.AddFrontTriggers());
                if (Game.IsAdvanced)
                {
                    base.AddSideTriggers(this.AddFrontAdvancedTriggers());
                }
            }
            //Back Triggers
            else
            {
                base.AddSideTriggers(this.AddDecapitatedTriggers());
                base.AddSideTrigger(base.AddCannotDealDamageTrigger((Card c) => c == base.Card));
                if (Game.IsAdvanced)
                {
                    base.AddSideTriggers(this.AddDecapitatedAdvancedTriggers());
                }
            };
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
                cardSource = base.GetCardSource(null);
            }
            IEnumerator coroutine = base.GameController.RemoveTarget(base.Card, cardSource: cardSource);
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
    }
}