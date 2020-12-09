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
            if (base.Game.IsAdvanced)
            {
                base.SpecialStringMaker.ShowIfElseSpecialString(() => base.Game.Journal.DealDamageEntriesThisRound().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Any(), () => "Phase has been dealt damage this turn.", () => "Phase has not been dealt damage this turn.");
            }
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                //Front
                //At the start of the villain turn, if there are 3 or more obstacles in play, flip {Phase}'s villain character cards.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsObstacle(c) && c.IsInPlayAndHasGameText)).Count() >= 3, base.FlipThisCharacterCardResponse, TriggerType.FlipCard));

                //Increase damage dealt by {Phase} by 1 for each obstacle that has been removed from the game.
                base.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard, (DealDamageAction action) => base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsObstacle(c) && c.IsOutOfGame)).Count()));

                //Phase is immune to damage dealt by environment cards.
                base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard && action.DamageSource.Card.IsEnvironment));

                //At the end of the villain turn, play the top card of the villain deck.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard));
                if (base.Game.IsAdvanced)
                {
                    //Front - Advanced
                    //When {Phase} is damaged, she becomes immune to damage until the end of the turn.
                    base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard && base.Game.Journal.DealDamageEntriesThisRound().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Any()));
                }
            }
            else
            {
                //Back
                //At the end of the villain turn, {Phase} deals each hero target {H} radiant damage. Then, flip {Phase}'s villain character cards.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage));
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine;
            if (base.Card.IsFlipped)
            {
                //Back
                //When {Phase} flips to this side, destroy the obstacle with the lowest HP and remove it from the game. If the card Insubstantial Matador is in play, destroy it.
                List<Card> list = new List<Card>();
                //...obstacle with the lowest HP...
                coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => this.IsObstacle(c), list, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //When {Phase} flips to this side, destroy the obstacle with the lowest HP 
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, list.FirstOrDefault());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //...and remove it from the game.
                coroutine = base.GameController.MoveCard(base.TurnTakerController, list.FirstOrDefault(), base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
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
                    coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing && c.IsHero), Game.H - 2, cardSource: base.GetCardSource());
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
                }
                yield break;
            }
        }

        private bool IsObstacle(Card c)
        {
            return c.DoKeywordsContain("obstacle");
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //{Phase} deals each hero target {H} radiant damage. 
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero, Game.H, DamageType.Radiant);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, flip {Phase}'s villain character cards.
            coroutine = base.GameController.FlipCard(this);
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