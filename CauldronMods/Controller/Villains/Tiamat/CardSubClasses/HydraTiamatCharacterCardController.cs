using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class HydraTiamatCharacterCardController : VillainCharacterCardController
    {
        protected HydraTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Face down heads are indestructible
        public override bool CanBeDestroyed
        {
            get
            {
                return false;
            }
        }

        protected abstract ITrigger[] AddFrontTriggers();

        public bool IsSpell(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "spell");
        }
        public bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head");
        }

        public override void AddSideTriggers()
        {
            //Element of Lightning Cancel Draws
            base.CannotDrawCards(ElementOfLightningCriteria);
            //Win Condition
            base.AddSideTrigger(base.AddTrigger<GameAction>(delegate (GameAction g)
            {
                if (base.GameController.HasGameStarted && !(g is GameOverAction) && !(g is IncrementAchievementAction))
                {
                    return base.FindCardsWhere((Card c) => c.IsFlipped && IsVillain(c) && c.DoKeywordsContain("head")).Count() == 6;
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
            }
            else
            {
                //Decapitated heads cannot deal damage
                base.AddSideTrigger(base.AddPreventDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.Card));
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

        private bool ElementOfLightningCriteria(TurnTakerController ttc)
        {
            if (ttc is HeroTurnTakerController httc)
            {
                return httc.CharacterCardControllers.Any(chc => chc.GetCardPropertyJournalEntryBoolean(ElementOfLightningCardController.PreventDrawPropertyKey) == true);
            }
            return false;
        }
    }
}
