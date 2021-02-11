using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TheChainConductorCardController : TerminusBaseCardController
    {
        /* 
         * Reveal cards from the top of your deck until you reveal a Memento or Equipment card. Put it into play or into 
         * your hand. Shuffle the rest of the revealed cards into your deck. 
         * If no card entered play this way, add 3 tokens to your Wrath pool and {Terminus} deals 1 target 2 cold damage.
         */
        public TheChainConductorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        Card _PlayedCard = null;

        public override IEnumerator Play()
        {
            var playStorage = new List<Card>();
            RemoveTemporaryTriggers();
            AddToTemporaryTriggerList(AddTrigger((CardEntersPlayAction cep) => cep.IsSuccessful && cep.CardSource != null && cep.CardSource.Card == this.Card, NoteEnteredPlayResponse, TriggerType.Hidden, TriggerTiming.After));

            // Reveal cards from the top of your deck until you reveal a Memento or Equipment card. Put it into play or into  your hand.
            IEnumerator coroutine = RevealCards_SelectSome_MoveThem_ReturnTheRest(DecisionMaker, DecisionMaker, TurnTaker.Deck, (Card c) => IsEquipment(c) || c.DoKeywordsContain("memento"), 1, 1, true, true, true, "equipment or memento");
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If no card entered play this way, 
            if (_PlayedCard == null)
            {
                // add 3 tokens to your Wrath pool 
                coroutine = AddWrathTokens(3);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                // and {Terminus} deals 1 target 2 cold damage.
                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Cold, 1, false, 1, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            _PlayedCard = null;
            yield break;
        }

        private IEnumerator NoteEnteredPlayResponse(CardEntersPlayAction cep)
        {
            _PlayedCard = cep.CardEnteringPlay;
            return DoNothing();
        }
    }
}
