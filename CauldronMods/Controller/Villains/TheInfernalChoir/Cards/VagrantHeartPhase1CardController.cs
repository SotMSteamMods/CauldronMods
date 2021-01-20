using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class VagrantHeartPhase1CardController : TheInfernalChoirUtilityCardController
    {
        public VagrantHeartPhase1CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(() => VagrantDeck).Condition = () => Card.IsInPlay;
        }

        public override bool CanBeDestroyed => false;

        public HeroTurnTaker VagrantTurnTaker => Card.Location.OwnerTurnTaker.IsHero ? Card.Location.OwnerTurnTaker.ToHero() : null;
        public Location VagrantDeck => VagrantTurnTaker?.Deck;

        /*
         * "This card is indestructible. Cards underneath this one are not in play and have no game text.",
           "When the Infernal Choir would be dealt damage, prevent that damage and instead move that many cards from the bottom of this Hero's deck beneath this card.",
           "If the Hero is Incapacitated or Vagrant Heart leaves play, the heroes lose. Game over."
         */

        public override void AddTriggers()
        {
            //NB: Trigger timing needs to be delayed till after Increase/Reduce, hence WouldBeDealtDamage instead of ImmuneToDamage
            AddTrigger<DealDamageAction>(dda => dda.Target == CharacterCard, VagrantHeartDamageReponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before);

            /*
             * "Whenever the Hero with the Vagrant Heart has no cards in it's deck...
             */
            AddTrigger<MoveCardAction>(mca => mca.Origin == VagrantDeck && mca.Destination != VagrantDeck && mca.WasCardMoved && !VagrantDeck.HasCards, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<PlayCardAction>(pca => pca.WasCardPlayed && pca.CardToPlay.Owner == VagrantTurnTaker && !VagrantDeck.HasCards, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<DrawCardAction>(dca => dca.DidDrawCard && dca.HeroTurnTaker == VagrantTurnTaker && !VagrantDeck.HasCards, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);
            AddTrigger<BulkMoveCardsAction>(bmca => VagrantDeck.HasCards, ga => GoToPhase2(ga), TriggerType.FlipCard, TriggerTiming.After);

            AddTrigger<TargetLeavesPlayAction>(tlpa => tlpa.TargetLeavingPlay.Owner == VagrantTurnTaker && VagrantTurnTaker.IsIncapacitatedOrOutOfGame, VagrantHeartGameOver, TriggerType.GameOver, TriggerTiming.After);

            base.AddTriggers();
        }

        /* ..shuffle all cards under Vagrant Heart back into the hero's deck, and flip {TheInfernalChoir}'s villain character cards and Vagrant Heart." */
        private IEnumerator GoToPhase2(GameAction action)
        {
            var coroutine = GameController.FlipCard(CharacterCardController, cardSource: CharacterCardController.GetCardSource());
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

            coroutine = GameController.ImmuneToDamage(action, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var vagrantHero = Card.Location.OwnerTurnTaker;
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
