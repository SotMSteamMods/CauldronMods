using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class GravitationalLensingCardController : CardController
    {
        public GravitationalLensingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, reveal the top 2 cards of your deck. Replace 1 and discard the other.",
            var topDeck = new MoveCardDestination(TurnTaker.Deck);
            var trash = new MoveCardDestination(TurnTaker.Trash);
            IEnumerator coroutine = RevealCardsFromTopOfDeck_DetermineTheirLocation(DecisionMaker, DecisionMaker, TurnTaker.Deck, topDeck, trash, 2, 1, 0);
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

        public override void AddTriggers()
        {
            //"Increase damage dealt by {Impact} by 1."
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.CharacterCard), 1);
        }
    }
}