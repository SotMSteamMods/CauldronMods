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
        protected abstract ITrigger[] AddDecapitatedChallengeTriggers();

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            AddTrigger((GameAction ga) => TurnTakerController is TiamatTurnTakerController tttc && !tttc.AreStartingCardsSetUp, (TurnTakerController as TiamatTurnTakerController).MoveStartingCards, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public override void AddSideTriggers()
        {
            //Win Condition
            base.AddSideTrigger(base.AddTrigger<GameAction>(delegate (GameAction g)
            {
                if (base.GameController.HasGameStarted && !(g is GameOverAction) && !(g is IncrementAchievementAction))
                {
                    return base.FindCardsWhere((Card c) => IsVillain(c) && c.IsInPlay && c.IsFlipped).Count() == 3;
                }
                return false;
            }, (GameAction g) => base.DefeatedResponse(g), new TriggerType[]
            {
                TriggerType.GameOver,
                TriggerType.Hidden
            }, TriggerTiming.After, outOfPlayTrigger: true));
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
                base.AddSideTrigger(base.AddPreventDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.Card));
                if (Game.IsAdvanced)
                {
                    base.AddSideTriggers(this.AddDecapitatedAdvancedTriggers());
                }
                if(Game.IsChallenge)
                {
                    base.AddSideTriggers(this.AddDecapitatedChallengeTriggers());
                }
            };
        }

        //When a {Tiamat} head is destroyed, flip her.
        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            IEnumerator coroutine = base.GameController.FlipCard(this, actionSource: destroyCard.ActionSource, cardSource: GetCardSource());
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

        protected IEnumerator ChallengeRestoreHeadResponse(GameAction ga)
        {
            //"Whenever a villain Spell card enters play, if the head it names is decapitated, flip that head... 
            IEnumerator coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and restore it to {H * 3} HP.",
            coroutine = GameController.MakeTargettable(this.Card, 40, H * 3, GetCardSource());
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