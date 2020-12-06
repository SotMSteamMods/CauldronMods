using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Cauldron.Celadroch
{
    public class CeladrochCharacterCardController : VillainCharacterCardController
    {
        /*
         * Black Wind Rising:
         * Gameplay:
         * Villain cards cannot be played.
		 * At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card.
		 * Otherwise, add 1 token to the storm pool.
         *
         * Advanced:
         * Reduce Damage Dealt to Relics by 1.
         *
         * The Dark Mountain
         * Gameplay:
         * At the start of the villain turn, add 1 token to the storm pool.
         * You may play the top card of the villain deck to remove 2 cards from the storm pool.
		 * Whenever an elemental, zombie, demon, or chosen is played from the top of the villain deck,
		 *   search the villain deck and trash for all other cards with the same keyword and put them into play.
		 *   Shuffle the villain deck.
		 * At the end of the villain turn, {Celadroch} deals the {H - 1} hero targets with the highest HP X projectile damage each, where X is the number of tokens in the storm pool minus 2.
		 * If Forsaken Crusader is in the villain trash, put it into play.
		 * If a villain card was not played this turn, destroy {H} hero ongoing and/or equipment cards.
         *
         * Advanced:
         * When {Celadroch} flips to this side, add 2 tokens to the storm pool.
         */

        public static readonly string StormPoolIdentifier = "StormPool";
        public static readonly string[] MinionKeywords = new[] { "elemental", "zombie", "demon", "chosen" };

        public TokenPool StormPool => CharacterCard.FindTokenPool(StormPoolIdentifier);

        public CeladrochCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TopCardSpecialString, null, () => new[] { FindCeladrochsTopCard() }).Condition = () => Card.IsInPlay && !Card.IsFlipped;
        }

        private Card FindCeladrochsTopCard()
        {
            return Game.Journal.CardPropertiesEntries(j => j.Key == "CeladrochsTopCard").FirstOrDefault()?.Card;
        }

        private string TopCardSpecialString()
        {
            var card = FindCeladrochsTopCard();
            if (card != null)
            {
                if (card == TurnTaker.Deck.TopCard)
                    return $"Celadroch's top card is {card.Title}";
                return $"Celadroch's top card was {card.Title}";
            }
            return null;
        }

        public override void AddSideTriggers()
        {
            // Front side (Black Wind Rising)
            if (!base.Card.IsFlipped)
            {
                //Villain Cards cannot be played
                CannotPlayCards(ttc => ttc.IsVillain && !ttc.CharacterCard.IsFlipped);

                //At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card. Otherwise, add 1 token to the storm pool.
                AddStartOfTurnTrigger(tt => tt == TurnTaker, FrontSideAddTokensOrFlip, new[] { TriggerType.AddTokensToPool, TriggerType.FlipCard });

                if (IsGameAdvanced)
                {
                    AddReduceDamageTrigger(c => c.IsTarget && c.IsRelic, 1);
                }
            }
            // Flipped side (The Dark Mountain)
            else
            {
                //At the start of the villain turn, add 1 token to the storm pool.
                AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.AddTokensToPool(StormPool, 1, GetCardSource()), TriggerType.AddTokensToPool);

                //You may play the top card of the villain deck to remove 2 cards from the storm pool.
                AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnOptionalCardPlayResponse, new[] { TriggerType.ModifyTokens, TriggerType.PlayCard });

                // Whenever an elemental, zombie, demon, or chosen is played from the top of the villain deck...
                Func<Card, bool> playCriteria = c => c.Owner == TurnTaker && c.GetKeywords().Intersect(MinionKeywords, StringComparer.Ordinal).Any();
                AddTrigger<PlayCardAction>(pca => !pca.IsPutIntoPlay && pca.Origin == TurnTaker.Deck && !pca.FromBottom && playCriteria(pca.CardToPlay),
                                            pca => SummonMinionsReponse(pca.CardToPlay), TriggerType.PutIntoPlay, TriggerTiming.After);

                //At the end of the villain turn, {Celadroch} deals the {H - 1} hero targets with the highest HP X projectile damage each, where X is the number of tokens in the storm pool minus 2.
                AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => c.IsHero && c.IsTarget,
                    targetType: TargetType.HighestHP,
                    amount: StormPool.CurrentValue >= 2 ? StormPool.CurrentValue - 2 : 0, //make sure we aren't dealing negative damage
                    damageType: DamageType.Projectile,
                    highestLowestRanking: 1,
                    numberOfTargets: H - 1);

                //If Forsaken Crusader is in the villain trash, put it into play.
                AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoNothing(), TriggerType.PlayCard);

                //If a villain card was not played this turn, destroy {H} hero ongoing and/or equipment cards.
                AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoNothing(), TriggerType.DestroyCard);

                base.AddDefeatedIfDestroyedTriggers();
            }
        }


        private IEnumerator FrontSideAddTokensOrFlip(PhaseChangeAction action)
        {
            //At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card. Otherwise, add 1 token to the storm pool.
            IEnumerator coroutine;
            if (StormPool.CurrentValue >= 3)
            {
                coroutine = FlipThisCharacterCardResponse(action);
            }
            else
            {
                coroutine = GameController.AddTokensToPool(StormPool, 1, GetCardSource());
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override bool CanBeDestroyed => base.CharacterCard.IsFlipped;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (base.Card.IsFlipped)
            {
                yield break;
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MakeTargettable(Card, Card.Definition.FlippedHitPoints.Value, Card.Definition.FlippedHitPoints.Value, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (IsGameAdvanced)
            {
                coroutine = GameController.AddTokensToPool(StormPool, 2, GetCardSource());
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

        private IEnumerator StartOfTurnOptionalCardPlayResponse(PhaseChangeAction action)
        {
            List<YesNoCardDecision> result = new List<YesNoCardDecision>();
            var coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.RemoveTokens, CharacterCard, storedResults: result, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayerAnswerYes(result))
            {
                coroutine = GameController.RemoveTokensFromPool(StormPool, 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(action);
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

        private IEnumerator SummonMinionsReponse(Card card)
        {
            var keyword = card.GetKeywords().Intersect(MinionKeywords, StringComparer.Ordinal).First();
            string msg = $"{TurnTaker.Name} summons forth his {keyword} minions.";

            var coroutine = GameController.SendMessageAction(msg, Priority.High, GetCardSource(), new[] { card }, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var cards = FindCardsWhere(c => (c.Location == TurnTaker.Deck || c.Location == TurnTaker.Trash) && c != card && c.DoKeywordsContain(keyword));
            coroutine = GameController.MoveCards(TurnTakerController, cards, TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
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
}