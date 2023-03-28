using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class TheInfernalChoirCharacterCardController : VillainCharacterCardController
    {

        /*
         * "setup": [
				"At the start of the game, put {TheInfernalChoir}'s villain character cards into play, “Unfinished Business“ side up.",
				"Search the villain deck for the card Vagrant Heart: Hidden Heart and play it face-up in the play area of a random hero.",
				"Put the card Vagrant Heart: Soul Revealed off to the side. Shuffle the villain deck."
			],
			"gameplay": [
				"(TheInfernalChoir) is indestructible.",
				"If the Hero with the Vagrant Heart in it's play area has no cards in it's deck, shuffle all cards under Vagrant Heart: Hidden Heart back into the hero's deck, flip {TheInfernalChoir}'s villain character cards and replace the Vagrant Heart: Hidden Heart with Vagrant Heart: Soul Revealed.",
				"At the end of the villain turn, play the top card of the villain deck. Then {TheInfernalChoir} deals each non-ghost, non-villain target 1 infernal damage."
			],
            "advanced": "Increase all damage dealt by 1.",
            "flippedGameplay": [
				"Redirect all hero damage that would be dealt to villain targets during the villain turn to the hero target with the highest HP.",
				"At the start of the villain turn, remove all but the bottom 5 cards of each hero's deck from the game, then destroy each hero character whose deck contains no cards. Play the top card of the villain deck and each hero deck in order in turn order. Remove all hero cards played this way from the game at the end of the villain turn."
			],
			"flippedAdvanced": "At the start of each player's turn, remove the top card of their deck from the game.",
         */

        public TheInfernalChoirCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIfSpecificCardIsInPlayUsingTitle(() => !Card.IsFlipped ? FindCard("VagrantHeartPhase1") : FindCard("VagrantHeartPhase2"));
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.").Condition = () => !Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => Card.IsFlipped;

            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == Card && !Card.IsFlipped)
                return true;

            return base.AskIfCardIsIndestructible(card);
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => PlayTheTopCardOfTheVillainDeckWithMessageResponse(pca), TriggerType.PlayCard));
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => !(IsGhost(c) || IsVillainTarget(c)), TargetType.All, 1, DamageType.Infernal));

                if (Game.IsAdvanced)
                {
                    AddSideTrigger(AddIncreaseDamageTrigger(dda => true, 1));
                }
            }
            else
            {
                AddSideTrigger(AddTrigger((DealDamageAction dda) => dda.DamageSource.IsHero && IsVillainTarget(dda.Target) && Game.ActiveTurnTaker == TurnTaker, RedirectToHighestHero, TriggerType.RedirectDamage, TriggerTiming.Before));
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => FlippedCardRemoval(pca), new[] { TriggerType.RemoveFromGame, TriggerType.PlayCard }));
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => FlippedRemovePlayedCards(pca), TriggerType.RemoveFromGame));

                if (Game.IsAdvanced)
                {
                    AddSideTrigger(AddStartOfTurnTrigger(tt => IsHero(tt), pca => AdvancedRemoveTopCardOfDeck(pca), TriggerType.RemoveFromGame));
                }

                //This trigger's only purpose is to force the GameController.WouldAutoDraw to consider there to be a candidate trigger.
                AddSideTrigger(AddTrigger<DrawCardAction>(dca => dca.HeroTurnTaker.Deck.NumberOfCards <= 5, dca => DoNothing(), TriggerType.Other, TriggerTiming.After));

                AddDefeatedIfDestroyedTriggers();
            }
        }

        protected bool IsGhost(Card c, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return c != null && (c.DoKeywordsContain("ghost", evenIfUnderCard, evenIfFaceDown) || GameController.DoesCardContainKeyword(c, "ghost", evenIfUnderCard, evenIfFaceDown));
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine;
            var p1Heart = TurnTaker.FindCard("VagrantHeartPhase1", false);
            var p2Heart = TurnTaker.FindCard("VagrantHeartPhase2", false);
            var tt = p1Heart.Location.OwnerTurnTaker;
            var httc = FindHeroTurnTakerController(tt.ToHero());

            //We have to preremove the Triggers from the first heart or else it triggers incorrectly.
            var p1Cc = FindCardController(p1Heart);
            p1Cc.RemoveAllTriggers();

            coroutine = GameController.ShuffleCardsIntoLocation(HeroTurnTakerController, p1Heart.UnderLocation.Cards, tt.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(TurnTakerController, p1Heart, TurnTaker.OutOfGame, flipFaceDown: true, evenIfIndestructible: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (p2Heart.IsFlipped)
                p2Heart.SetFlipped(false);

            coroutine = GameController.MoveIntoPlay(TurnTakerController, p2Heart, tt, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(Game.IsChallenge)
            {
                //CHALLENGE: When {TheInfernalChoir} flips, reduce damage dealt to {TheInfernalChoir} by 2 until the start of the villain turn.

                ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(2);
                effect.TargetCriteria.IsSpecificCard = CharacterCard;
                effect.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(effect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

            }


            coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator FlippedCardRemoval(GameAction action)
        {
            IEnumerator coroutine;
            foreach (var httc in GameController.FindHeroTurnTakerControllers().Where(ht => !ht.IsIncapacitatedOrOutOfGame))
            {
                var htt = httc.HeroTurnTaker;
                if (htt.Deck.NumberOfCards > 5)
                {
                    var cards = htt.Deck.GetTopCards(htt.Deck.NumberOfCards - 5).ToList();
                    int count = cards.Count;
                    var msg = $"The terrible song of {Card.Title} removes {count} {count.ToString_SingularOrPlural("card", "cards")} from {htt.Deck.GetFriendlyName()}.";
                    coroutine = GameController.SendMessageAction(msg, Priority.Medium, GetCardSource(), showCardSource: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.BulkMoveCards(TurnTakerController, cards, htt.OutOfGame, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else if (!htt.Deck.HasCards)
                {
                    var msg = $"{htt.NameRespectingVariant} can no longer resist {Card.Title}'s spectral music....{{BR}}" +
                              $"[b]{htt.CharacterCards.Select(c => c.AlternateTitleOrTitle).ToCommaList(true)} will be destroyed.[/b]";
                    coroutine = GameController.SendMessageAction(msg, Priority.Critical, GetCardSource(), showCardSource: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.DestroyCards(httc, new LinqCardCriteria(c =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && htt.CharacterCards.Contains(c)),
                                autoDecide: true,
                                cardSource: GetCardSource());
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

            List<Card> playedCards = new List<Card>();
            coroutine = DoActionToEachTurnTakerInTurnOrder(ttc => !ttc.IsIncapacitatedOrOutOfGame && !ttc.TurnTaker.IsEnvironment && GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()), ttc => GameController.PlayTopCardOfLocation(ttc, ttc.TurnTaker.Deck, cardSource: GetCardSource(), playedCards: playedCards, showMessage: true));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            foreach (var card in playedCards)
            {
                if (IsHero(card))
                    Journal.RecordCardProperties(card, "TheInfernalChoirRemoveFromGame", true);
            }
        }

        private IEnumerator FlippedRemovePlayedCards(GameAction action)
        {
            var cards = FindCardsWhere((Card c) => !c.IsOutOfGame && Journal.GetCardPropertiesBoolean(c, "TheInfernalChoirRemoveFromGame") != null && Journal.GetCardPropertiesBoolean(c, "TheInfernalChoirRemoveFromGame")  == true, visibleToCard: GetCardSource());
            var coroutine = GameController.MoveCards(TurnTakerController, cards, c => new MoveCardDestination(c.Owner.OutOfGame, showMessage: true), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AdvancedRemoveTopCardOfDeck(PhaseChangeAction action)
        {
            var tt = action.ToPhase.TurnTaker;

            if(!tt.Deck.HasCards)
            {
                yield break;
            }

            var coroutine = GameController.MoveCard(TurnTakerController, tt.Deck.TopCard, tt.OutOfGame, showMessage: true, actionSource: action, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RedirectToHighestHero(DealDamageAction dealDamage)
        {
          
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine2 = base.GameController.FindTargetWithHighestHitPoints(1, (Card card) => IsHero(card) && base.GameController.IsCardVisibleToCardSource(card, GetCardSource()), storedResults, dealDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            if (storedResults.Count() > 0)
            {
                Card newTarget = storedResults.FirstOrDefault();
                IEnumerator coroutine3 = base.GameController.RedirectDamage(dealDamage, newTarget, isOptional: false, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
            }
        }

        public void ShowIfSpecificCardIsInPlayUsingTitle(Func<Card> card)
        {
            Func<string> output = delegate
            {
                string text = (card().IsInPlayAndHasGameText ? "" : "not ");
                return card().Title + " is " + text + "in play.";
            };
            SpecialStringMaker.ShowSpecialString(output);
        }
    }
}
