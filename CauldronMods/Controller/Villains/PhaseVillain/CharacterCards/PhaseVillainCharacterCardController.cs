using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class PhaseVillainCharacterCardController : VillainCharacterCardController
    {
        public PhaseVillainCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsObstacle(c), "obstacle"));

            if (base.Game.IsAdvanced)
            {
                base.SpecialStringMaker.ShowIfElseSpecialString(() => base.Game.Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Any(), () => "Phase has been dealt damage this turn.", () => "Phase has not been dealt damage this turn.");
            }

            SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria(c => IsObstacle(c) && c.IsOutOfGame,
                        singular: "obstacle card removed from the game",
                        plural: "obstacle cards removed from the game"));
            SpecialStringMaker.ShowListOfCardsOutOfGame(new LinqCardCriteria((Card c) => c.Owner == TurnTaker, "Phase"), () => true).Condition = () => FindCardsWhere(c => c.IsOutOfGame && c.Owner == TurnTaker).Any();
            SpecialStringMaker.ShowSpecialString(() => $"Damage dealt by {CharacterCard.Title} increased by {ObstaclesRemovedFromGame()}.", () => true)
                .Condition = () => !base.CharacterCard.IsFlipped && ObstaclesRemovedFromGame() > 0;
        }

        private int ObstaclesRemovedFromGame()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsObstacle(c) && c.IsOutOfGame)).Count();
        }

        public override void AddSideTriggers()
        {
            if (!base.CharacterCard.IsFlipped)
            {
                //Front
                //At the start of the villain turn, if there are 3 or more obstacles in play, flip {Phase}'s villain character cards.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsObstacle(c) && c.IsInPlayAndHasGameText)).Count() >= 3, base.FlipThisCharacterCardResponse, TriggerType.FlipCard));

                //Increase damage dealt by {Phase} by 1 for each obstacle that has been removed from the game.
                base.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard, (DealDamageAction action) => ObstaclesRemovedFromGame()));

                //Phase is immune to damage dealt by environment cards.
                base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card.IsEnvironment));

                //At the end of the villain turn, play the top card of the villain deck.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard));
                if (base.Game.IsAdvanced)
                {
                    //Front - Advanced
                    //When {Phase} is damaged, she becomes immune to damage until the end of the turn.
                    base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard && base.Game.Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Any()));
                }
            }
            if (base.CharacterCard.IsFlipped)
            {
                //Back
                //At the end of the villain turn, {Phase} deals each hero target {H} radiant damage. Then, flip {Phase}'s villain character cards.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage));
            }

            if(Game.IsChallenge)
            {
                //Whenever an Obstacle is destroyed, Obstacles become immune to damage until the end of the turn.
                base.AddSideTrigger(base.AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && IsObstacle(dca.CardToDestroy.Card), ChallengeImmuneToDamageResponse, TriggerType.ImmuneToDamage, TriggerTiming.After));
            }

            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator ChallengeImmuneToDamageResponse(DestroyCardAction dca)
        {
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.TargetCriteria.HasAnyOfTheseKeywords = new List<string>() { "obstacle" };
            effect.UntilThisTurnIsOver(Game);

            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            base.RemoveSideTriggers();
            this.AddSideTriggers();
            IEnumerator coroutine;
            if (base.Card.IsFlipped)
            {
                //Back
                //When {Phase} flips to this side, destroy the obstacle with the lowest HP and remove it from the game. If the card Insubstantial Matador is in play, destroy it.
                List<Card> list = new List<Card>();
                //...obstacle with the lowest HP...
                coroutine = base.GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => this.IsObstacle(c), list, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                var lowest = list.FirstOrDefault();
                //When {Phase} flips to this side, destroy and remove from the game the obstacle with the lowest HP 
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, lowest,
                                overrideOutput: $"{CharacterCard.Title} removes {lowest.Title} from the game.",
                                showOutput: true,
                                overrideDestroyLocation: TurnTaker.OutOfGame,
                                cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //If the card Insubstantial Matador is in play, destroy it.
                Card matador = base.FindCard("InsubstantialMatador");
                if (matador.IsInPlayAndHasGameText)
                {
                    coroutine = base.GameController.DestroyCard(this.DecisionMaker, matador);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                if (base.Game.IsAdvanced)
                {
                    //Back - Advanced
                    //When {Phase} flips to this side, destroy {H - 2} hero ongoing cards.
                    coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && IsHero(c), "hero ongoing"), Game.H - 2, cardSource: base.GetCardSource());
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

            yield break;
        }

        private bool IsObstacle(Card c)
        {
            return c.DoKeywordsContain("obstacle");
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //{Phase} deals each hero target {H} radiant damage. 
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHero(c), Game.H, DamageType.Radiant);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, flip {Phase}'s villain character cards.
            coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
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