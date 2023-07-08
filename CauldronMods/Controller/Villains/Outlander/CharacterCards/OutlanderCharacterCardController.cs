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
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            Card.UnderLocation.OverrideIsInPlay = false;
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt damage this turn.", () => "Outlander has not been dealt damage this turn.").Condition = () => Card.IsFlipped;
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsTrace(c), "trace"));
        }

        protected const string OncePerTurn = "OutlanderFlippedOncePerTurn";
        private ITrigger ReduceDamageTrigger;

        public override void AddSideTriggers()
        {
            if (base.CharacterCard.IsFlipped)
            {
                //Back:
                //Reduce the first damage dealt to {Outlander} each turn by {H}.
                this.ReduceDamageTrigger = base.AddTrigger<DealDamageAction>((DealDamageAction action) => !HasBeenSetToTrueThisTurn(OncePerTurn) && action.Target == this.Card, this.ReduceDamageResponse, TriggerType.ReduceDamageOneUse, TriggerTiming.Before);
                base.AddSideTrigger(this.ReduceDamageTrigger);
                if (base.Game.IsAdvanced)
                {
                    //Advanced:
                    //At the end of the villain turn, destroy {H - 2} hero ongoing and/or equipment cards.
                    base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyCardsResponse, TriggerType.DestroyCard));
                }
                base.AddDefeatedIfDestroyedTriggers();
            }

            if(Game.IsChallenge)
            {
                //CHALLENGE: Whenever a villain Ongoing is destroyed, other villain Ongoings are indestructible until the end of the villain turn.
                base.AddSideTrigger(AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && IsOngoing(dca.CardToDestroy.Card) && IsVillain(dca.CardToDestroy.Card), ChallengeOngoingDestroyedResponse, TriggerType.CreateStatusEffect, TriggerTiming.After));
            }


        }

        private IEnumerator ChallengeOngoingDestroyedResponse(DestroyCardAction dca)
        {
            var challengeStatusEffect = new MakeIndestructibleStatusEffect();
            challengeStatusEffect.CardsToMakeIndestructible.IsVillain = true;
            challengeStatusEffect.CardsToMakeIndestructible.HasAnyOfTheseKeywords = new List<string> { "ongoing" };
            challengeStatusEffect.CardsToMakeIndestructible.IsNotSpecificCard = dca.CardToDestroy.Card;
            challengeStatusEffect.CardsToMakeIndestructible.OutputString = "Villain ongoing cards";
            challengeStatusEffect.ToTurnPhaseExpiryCriteria.Phase = Phase.End;
            challengeStatusEffect.ToTurnPhaseExpiryCriteria.TurnTaker = this.TurnTaker;
            IEnumerator coroutine = AddStatusEffect(challengeStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
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
            var coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!CharacterCard.IsFlipped)
            {
                if (Game.IsAdvanced)
                {
                    //Front - Advanced:
                    //Whenever {Outlander} flips to this side, he becomes immune to damage until the start of the next villain turn.
                    var statusEffect = new ImmuneToDamageStatusEffect();
                    statusEffect.TargetCriteria.IsSpecificCard = CharacterCard;
                    statusEffect.UntilStartOfNextTurn(TurnTaker);
                    statusEffect.CardSource = Card;

                    coroutine = GameController.AddStatusEffect(statusEffect, true, GetCardSource());
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
            else
            {
                //When {Outlander} flips to this side, restore him to 20 HP...
                coroutine = GameController.SetHP(Card, 20, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //...destroy all copies of Anchored Fragment...
                coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "AnchoredFragment" && c.IsInPlayAndHasGameText), autoDecide: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //...and put a random Trace into play.
                var trace = CharacterCard.UnderLocation.Cards.TakeRandomFirstOrDefault(Game.RNG);
                if (trace != null)
                {
                    coroutine = GameController.PlayCard(TurnTakerController, trace, isPutIntoPlay: true, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
                if (FindCardsWhere(new LinqCardCriteria((Card c) => IsTrace(c) && c.IsInPlayAndHasGameText)).Count() < Game.H)
                {
                    coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
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
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Cards beneath this one are not considered in play. Trace cards are indestructible.
            if(IsTrace(card))
            {
                return true;
            }
            return false;
        }

        public override bool CanBeDestroyed => Card.IsFlipped;
        public readonly string OngoingDestroyedLastTurnKey = "OngoingDestroyedLastTurn";


        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (!Card.IsFlipped)
            {
                //Front:
                //When {Outlander} would be destroyed instead flip his villain character cards.
                IEnumerator coroutine = GameController.FlipCard(this, actionSource: destroyCard.ActionSource, cardSource: GetCardSource());
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

        private IEnumerator DestroyCardsResponse(PhaseChangeAction action)
        {
            //...destroy {H - 2} hero ongoing and/or equipment cards.
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), Game.H - 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool IsTrace(Card c)
        {
            return c.DoKeywordsContain("trace");
        }
    }
}
