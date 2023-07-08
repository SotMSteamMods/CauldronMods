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
            SpecialStringMaker.ShowTokenPool(base.CharacterCard.FindTokenPool(StormPoolIdentifier)).Condition = () => Card.IsInPlay && !Card.IsFlipped;
            SpecialStringMaker.ShowSpecialString(TopCardSpecialString, null, () => new[] { FindCeladrochsTopCard() }).Condition = () => Game.HasGameStarted && Card.IsInPlay && !Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: Game.H - 1).Condition = () => Card.IsInPlay && Card.IsFlipped;
            SpecialStringMaker.ShowIfElseSpecialString(() => Game.Journal.CardEntersPlayEntriesThisTurn().Any(e => e.Card.IsVillain), () => "A villain card has entered play this turn.", () => "A villain card has not entered play this turn.").Condition = () => Card.IsInPlay && Card.IsFlipped && Game.ActiveTurnTaker == TurnTaker;
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

        public override void AddTriggers()
        {
            //Villain Cards cannot be played
            CannotPlayCards(ttc => ttc.IsVillain && !ttc.CharacterCard.IsFlipped);

            base.AddTriggers();
        }

        public override void AddSideTriggers()
        {
            // Front side (Black Wind Rising)
            if (!base.Card.IsFlipped)
            {
                //At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card. Otherwise, add 1 token to the storm pool.
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, FrontSideAddTokensOrFlip, new[] { TriggerType.AddTokensToPool, TriggerType.FlipCard }));

                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddReduceDamageTrigger(c => c.IsTarget && c.IsRelic, 1));
                }
            }
            // Flipped side (The Dark Mountain)
            else
            {
                //At the start of the villain turn, add 1 token to the storm pool.
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.AddTokensToPool(StormPool, 1, GetCardSource()), TriggerType.AddTokensToPool));

                //You may play the top card of the villain deck to remove 2 tokens from the storm pool.
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnOptionalCardPlayResponse, new[] { TriggerType.ModifyTokens, TriggerType.PlayCard }));

                // Whenever an elemental, zombie, demon, or chosen is played from the top of the villain deck...
                AddSideTrigger(AddTrigger<CompletedCardPlayAction>(SummonMinionCriteria,
                                            SummonMinionsReponse, TriggerType.PutIntoPlay, TriggerTiming.After));

                //At the end of the villain turn, {Celadroch} deals the {H - 1} hero targets with the highest HP X projectile damage each, where X is the number of tokens in the storm pool minus 2.
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => IsHeroTarget(c),
                    targetType: TargetType.HighestHP,
                    amount: 0,
                    damageType: DamageType.Projectile,
                    highestLowestRanking: 1,
                    numberOfTargets: H - 1,
                    dynamicAmount: c => StormPool.CurrentValue >= 2 ? StormPool.CurrentValue - 2 : 0)); //make sure we aren't dealing negative damage

                //If Forsaken Crusader is in the villain trash, put it into play.
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => PlayForsakenCrusaderFromTrash(), TriggerType.PutIntoPlay));

                //If a villain card was not played this turn, destroy {H} hero ongoing and/or equipment cards.
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => NoCardsPlayedResponse(), TriggerType.DestroyCard));

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
                coroutine = GameController.SendMessageAction($"{Card.Title} adds a token to his {StormPool.Name}.", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

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

            if (Card.IsFlipped)
            {
                //coroutine = GameController.ChangeMaximumHP(Card, Card.Definition.FlippedHitPoints.Value, true, GetCardSource());
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

                var msg = GameController.SendMessageAction($"{Card.Title} removes 2 tokens from the Storm Pool and plays the top card of the villain deck...", Priority.High, GetCardSource());
                coroutine = PlayTheTopCardOfTheVillainDeckResponse(action);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msg);
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msg);
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private bool SummonMinionCriteria(CompletedCardPlayAction pca)
        {
            if (!pca.IsPutIntoPlay && pca.Origin == TurnTaker.Deck && !pca.PlayedFromBottom)
            {
                var c = pca.CardPlayed;
                if (c.Owner == TurnTaker && c.GetKeywords().Intersect(MinionKeywords, StringComparer.Ordinal).Any())
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator SummonMinionsReponse(CompletedCardPlayAction pca)
        {
            var card = pca.CardPlayed;

            Console.WriteLine($"TRIGGER - {card.Title}, {card.Location.GetFriendlyName()}");

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


            IEnumerable<Card> minionsToSummon = FindCardsWhere(c => (c.Location == TurnTaker.Deck || c.Location == TurnTaker.Trash) && c.DoKeywordsContain(keyword));
            coroutine = GameController.PlayCards(DecisionMaker,
                c => minionsToSummon.Contains(c), false, true,
                allowAutoDecide: true,
                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Note: using the GameAction shuffleLocation screws something with the card being played.  This doesn't.
            TurnTaker.Deck.ShuffleCards();
        }

        private IEnumerator PlayForsakenCrusaderFromTrash()
        {
            Card card = FindCard(ForsakenCrusaderCardController.Identifier);
            if (card.IsInTrash)
            {
                var coroutine = ReviveForsakenCrusaderMessage(card);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayCard(TurnTakerController, card, isPutIntoPlay: true, cardSource: GetCardSource());
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

        private IEnumerator ReviveForsakenCrusaderMessage(Card card)
        {
            int rng = Game.RNG.Next(1, 21);
            string msg = $"Your service has not ended...";
            bool showCardSource = true;
            if (5 <= rng && rng < 10)
            {
                msg = $"Arise"; showCardSource = true;
            }
            if (rng < 15)
            {
                msg = $"\"When can I slumber...\""; showCardSource = false;
            }
            if (rng < 20)
            {
                msg = $"\"I serve.\""; showCardSource = false;
            }
            if (rng == 21)
            {
                msg = $"Whose the best strong boi, that's right, it's you."; showCardSource = true;
            }

            return GameController.SendMessageAction(msg, Priority.Medium, GetCardSource(), new[] { card }, showCardSource);
        }

        private IEnumerator NoCardsPlayedResponse()
        {
            bool villainCardPlayed = Game.Journal.CardEntersPlayEntriesThisTurn().Any(e => e.Card.IsVillain);
            if (!villainCardPlayed)
            {
                var coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => IsHero(c) && (IsOngoing(c) || IsEquipment(c)) && c.IsInPlayAndNotUnderCard, "hero equipment or ongoing"), H, cardSource: GetCardSource());
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
}