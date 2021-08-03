using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class VagrantHeartPhase1CardController : TheInfernalChoirUtilityCardController
    {
        public VagrantHeartPhase1CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(() => VagrantDeck).Condition = () => Card.IsInPlay && VagrantDeck != null;
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");

            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            Card.UnderLocation.OverrideIsInPlay = false;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == Card)
                return true;

            return base.AskIfCardIsIndestructible(card);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => d.SelectedCard == base.Card || Card.UnderLocation.HasCard(d.SelectedCard));
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if ((card == base.Card || Card.UnderLocation.HasCard(card)) && !cardSource.Card.IsVillain)
            {
                return false;
            }
            return base.AskIfCardIsVisibleToCardSource(card, cardSource); ;
        }

        public override bool AskIfActionCanBePerformed(GameAction action)
        {
            bool? effected = action.DoesFirstCardAffectSecondCard((Card c) => !c.IsVillain, (Card c) => c == base.Card || Card.UnderLocation.HasCard(c));
            if (effected == true)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(action);
        }

        public HeroTurnTaker VagrantTurnTaker => Card.Location.OwnerTurnTaker.IsHero ? Card.Location.OwnerTurnTaker.ToHero() : null;
        public Location VagrantDeck => VagrantTurnTaker?.Deck;

        /*
         * "This card is indestructible. Cards underneath this one are not in play and have no game text.",
           "When the Infernal Choir would be dealt damage, prevent that damage and instead move that many cards from the bottom of this Hero's deck beneath this card.",
           "If the Hero is Incapacitated or Vagrant Heart leaves play, the heroes lose. Game over."
         */

        public override void AddTriggers()
        {
            //using inbuilt PreventDamageTrigger should handle conflicting cancel responses
            AddPreventDamageTrigger(dda => dda.Target == CharacterCard, VagrantHeartDamageReponse, new List<TriggerType> { TriggerType.MoveCard }, isPreventEffect: true);

            /*
             * "Whenever the Hero with the Vagrant Heart has no cards in it's deck...
             * I exclude Revealed Locations from the triggers since every move into a revealed will be matched with one of the other actions.
             * Cricket's Reveal and Replace card shouldn't trigger the flip unless that card is also played.
             */

            AddTrigger<MoveCardAction>(HeartMoveCriteria, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<PlayCardAction>(HeartPlayCriteria, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<DrawCardAction>(HeartDrawCriteria, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<BulkMoveCardsAction>(HeartBulkMoveCriteria, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);

            // "If the Hero is Incapacitated or Vagrant Heart leaves play, the heroes lose. Game over."
            AddTrigger<TargetLeavesPlayAction>(tlpa => tlpa.TargetLeavingPlay.Owner == VagrantTurnTaker && VagrantTurnTaker.IsIncapacitatedOrOutOfGame, VagrantHeartGameOver, TriggerType.GameOver, TriggerTiming.After);

            //visibility
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && !md.CardSource.Card.IsVillain, this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);

            base.AddTriggers();
        }

        private bool HeartMoveCriteria(MoveCardAction mca)
        {
            return mca.WasCardMoved && mca.Origin.OwnerTurnTaker == VagrantTurnTaker && !mca.Destination.IsRevealed && !VagrantDeck.HasCards;
        }

        private bool HeartPlayCriteria(PlayCardAction pca)
        {
            return pca.WasCardPlayed && pca.CardToPlay.Owner == VagrantTurnTaker && !VagrantTurnTaker.Revealed.HasCards && !VagrantDeck.HasCards;
        }

        private bool HeartDrawCriteria(DrawCardAction dca)
        {
            return dca.DidDrawCard && dca.HeroTurnTaker == VagrantTurnTaker && !VagrantDeck.HasCards;
        }

        private bool HeartBulkMoveCriteria(BulkMoveCardsAction bmca)
        {
            return !VagrantTurnTaker.Revealed.HasCards && !VagrantDeck.HasCards;
        }

        /* ..shuffle all cards under Vagrant Heart back into the hero's deck, and flip {TheInfernalChoir}'s villain character cards and Vagrant Heart." */
        private IEnumerator GoToPhase2(GameAction action)
        {
            var coroutine = GameController.FlipCard(CharacterCardController, actionSource: action, cardSource: CharacterCardController.GetCardSource(), allowBackToFront: false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator VagrantHeartGameOver(TargetLeavesPlayAction action)
        {
            var coroutine = GameController.GameOver(EndingResult.AlternateDefeat, $"The Vagrant Heart consumes {VagrantTurnTaker.Name}! [b]The heroes are defeated.[/b]", actionSource: action, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected IEnumerator VagrantHeartDamageReponse(DealDamageAction action)
        {
            IEnumerator coroutine;
            int amount = action.Amount; //NB: ImmuneToDamage call reduces amount to zero, store value ahead of time.

            //no need to prevent the damage ourselves, this is now a response to the damage having been prevented

            var vagrantHero = VagrantTurnTaker;
            if(vagrantHero is null)
            {
                yield break;
            }

            int numCardsBeingMoved = Math.Min(Card.Location.OwnerTurnTaker.Deck.NumberOfCards, amount);
            coroutine = GameController.SendMessageAction($"The damage to {CharacterCard.Title} is prevented and {numCardsBeingMoved} {GenericStringExtensions.ToString_CardOrCards(numCardsBeingMoved)} {GenericStringExtensions.ToString_IsOrAre(numCardsBeingMoved)} moved from the bottom of {Card.Location.OwnerTurnTaker.Deck.GetFriendlyName()} to beneath this card.", Priority.Medium, GetCardSource(), showCardSource: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            while (amount > 0 && vagrantHero.Deck.NumberOfCards > 0 && Card.IsInPlayAndHasGameText)
            {
                //Card.IsInPlayAndHasGameText to check that the Phase1 card hasn't been moved out of play.
                amount--;
                coroutine = GameController.MoveCard(TurnTakerController, vagrantHero.Deck.BottomCard, Card.UnderLocation, playCardIfMovingToPlayArea: false, actionSource: null, cardSource: GetCardSource());
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
}
