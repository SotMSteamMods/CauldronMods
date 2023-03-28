using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class DistributedHivemindSwarmEaterCharacterCardController : VillainCharacterCardController
    {
        public DistributedHivemindSwarmEaterCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => this.IsNanomutant(c), "nanomutant"));
            if (base.IsGameAdvanced)
            {
                base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            }
        }

        private bool IsNanomutant(Card c)
        {
            return c.DoKeywordsContain("nanomutant");
        }

        public override void AddStartOfGameTriggers()
        {
            //needed so that the challenge double-effect persists through save/load
            base.AddStartOfGameTriggers();
            if (IsGameChallenge)
            {
                foreach (Card c in TurnTaker.GetAllCards())
                {
                    if (c.IsUnderCard && IsNanomutant(c.Location.OwnerCard))
                    {
                        var controller = FindCardController(c);
                        if (controller is AugCardController augCC)
                        {
                            augCC.AddAbsorbTriggers(this.Card);
                        }
                    }
                }
            }
        }
        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                //Whenever a villain target would deal damage to another villain target, redirect that damage to the hero target with the highest HP.
                base.AddSideTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsVillainTarget && IsVillainTarget(action.Target) && action.DamageSource.Card != action.Target, this.RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before));
                //When a villain target enters play, flip {SwarmEater}'s villain character cards.
                //When {SwarmEater} flips to [Back] side, discard cards from the top of the villain deck until a target is discarded.
                //Put the discarded target beneath the villain target that just entered play. Then flip {SwarmEater}'s character cards.
                base.AddSideTrigger(base.AddTrigger<PlayCardAction>((PlayCardAction action) => IsVillainTarget(action.CardToPlay) && action.WasCardPlayed, this.AugmentTargetResponse, TriggerType.FlipCard, TriggerTiming.After));
                //At then end of the villain turn, if there are no nanomutants in play, play the top card of the villain deck.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && !base.GameController.FindTargetsInPlay((Card c) => this.IsNanomutant(c)).Any(), base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard));
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction action)
        {
            //Whenever a villain target would deal damage to another villain target, redirect that damage to the hero target with the highest HP.
            IEnumerator coroutine = base.RedirectDamage(action, TargetType.HighestHP, (Card c) => IsHero(c));
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
            //Front - Advanded:
            //Single-Minded Pursuit is indestructible.
            return !base.CharacterCard.IsFlipped && base.IsGameAdvanced && base.FindCardController(card) is SingleMindedPursuitCardController;
        }

        private IEnumerator AugmentTargetResponse(PlayCardAction action)
        {
            //When a villain target enters play, flip {SwarmEater}'s villain character cards.
            IEnumerator coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<RevealCardsAction> storedResult = new List<RevealCardsAction>();
            //When {SwarmEater} flips to this side, discard cards from the top of the villain deck until a target is discarded.
            coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, (Card c) => this.IsNanomutant(c), 1, storedResult, RevealedCardDisplay.ShowMatchingCards, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var absorbedCard = storedResult.FirstOrDefault()?.RevealedCards.LastOrDefault();
            if (absorbedCard != null && IsNanomutant(absorbedCard))
            {
                //Put the discarded target beneath the villain target that just entered play.
                coroutine = base.GameController.MoveCard(base.TurnTakerController, absorbedCard, action.CardToPlay.UnderLocation);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                ActivateAbsorbTriggers(absorbedCard);
            }

            coroutine = base.CleanupRevealedCards(base.TurnTaker.Revealed, base.TurnTaker.Trash);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then flip {SwarmEater}'s character cards.
            coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
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
            //Back - Advanced:
            if (base.CharacterCard.IsFlipped && base.Game.IsAdvanced)
            {
                //When {SwarmEater} flips to this side he regains {H - 2} HP.
                IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, Game.H - 2, cardSource: base.GetCardSource());
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

        private void ActivateAbsorbTriggers(Card absorbedCard)
        {
            CardController absorbedCC = base.FindCardController(absorbedCard);
            absorbedCC?.RemoveAllTriggers();
            if (absorbedCC != null && absorbedCC is AugCardController augCC)
            {
                base.GameController.RemoveInhibitor(absorbedCC);
                if(IsGameChallenge)
                {
                    //Challenge: Activated absorb texts also apply to Swarm Eater.
                    augCC.AddAbsorbTriggers(this.Card);
                }
                augCC.AddAbsorbTriggers();
            }
        }
    }
}