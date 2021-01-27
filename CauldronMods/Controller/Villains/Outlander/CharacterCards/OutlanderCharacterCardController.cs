using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class OutlanderCharacterCardController : VillainCharacterCardController
    {
        public OutlanderCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            base.CharacterCard.UnderLocation.OverrideIsInPlay = false;
            if (base.CharacterCard.IsFlipped)
            {
                base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt damage this turn.", () => "Outlander has not been dealt damage this turn.");
            }
        }

        protected const string OncePerTurn = "OncePerTurn";
        private ITrigger ReduceDamageTrigger;

        public override void AddSideTriggers()
        {
            if (base.CharacterCard.IsFlipped)
            { //Back:
              //Reduce the first damage dealt to {Outlander} each turn by {H}.
                this.ReduceDamageTrigger = base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.Target == this.Card, this.ReduceDamageResponse, TriggerType.ReduceDamageOneUse, TriggerTiming.Before);
                base.AddSideTrigger(this.ReduceDamageTrigger);
                if (base.Game.IsAdvanced)
                { //Advanced:
                    //At the end of the villain turn, destroy {H - 2} hero ongoing and/or equipment cards.
                    base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyCardsResponse, TriggerType.DestroyCard));
                }
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction action)
        {
            //Reduce the first damage dealt to {Outlander} each turn by {H}.
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            IEnumerator coroutine = base.GameController.ReduceDamage(action, base.Game.H, this.ReduceDamageTrigger, base.GetCardSource());
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

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine;
            if (!base.CharacterCard.IsFlipped)
            {
                if (base.Game.IsAdvanced)
                { //Front - Advanced:
                    //Whenever {Outlander} flips to this side, he becomes immune to damage until the start of the next villain turn.
                    ImmuneToDamageStatusEffect statusEffect = new ImmuneToDamageStatusEffect();
                    statusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
                    statusEffect.UntilStartOfNextTurn(base.TurnTaker);

                    coroutine = base.GameController.AddStatusEffect(statusEffect, true, new CardSource(this));
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
            else
            {
                //When {Outlander} flips to this side, restore him to 20 HP...
                coroutine = base.GameController.SetHP(base.CharacterCard, 20);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...destroy all copies of Anchored Fragment...
                coroutine = base.GameController.DestroyCards(base.DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "AnchoredFragment" && c.IsInPlayAndHasGameText));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and put a random Trace into play.
                coroutine = base.GameController.PlayCard(base.TurnTakerController, base.CharacterCard.UnderLocation.Cards.TakeRandomFirstOrDefault(base.GameController.Game.RNG), true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
                if (base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsTrace(c) && c.IsInPlayAndHasGameText)).Count() < base.Game.H)
                {
                    coroutine = base.GameController.FlipCard(this);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            coroutine = base.AfterFlipCardImmediateResponse();
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

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Cards beneath this one are not considered in play. Trace cards are indestructible.
            return this.IsTrace(card);
        }

        public override bool CanBeDestroyed
        {
            get
            {
                return base.CharacterCard.IsFlipped;
            }
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (!base.Card.IsFlipped)
            {
                //Front:
                //When {Outlander} would be destroyed instead flip his villain character cards.
                IEnumerator coroutine = base.GameController.FlipCard(this, actionSource: destroyCard.ActionSource, cardSource: base.GetCardSource());
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

        private IEnumerator DestroyCardsResponse(PhaseChangeAction action)
        {
            //...destroy {H - 2} hero ongoing and/or equipment cards.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c))), base.Game.H - 2, cardSource: base.GetCardSource());
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

        private bool IsTrace(Card c)
        {
            return c.DoKeywordsContain("trace");
        }
    }
}
