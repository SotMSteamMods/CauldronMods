using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Gray
{
    public class CriticalMassCardController : CardController
    {
        public CriticalMassCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new List<Location>() { base.TurnTaker.Deck, base.TurnTaker.Trash }, new LinqCardCriteria((Card c) => c.Identifier == "ChainReaction", "chain reaction"));
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "UnstableIsotope", "unstable isotope"));
        }
        public override IEnumerator Play()
        {
            //Search the villain deck and trash for all copies of Chain Reaction and put them into play.
            IEnumerator coroutine = base.PlayCardsFromLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.Identifier == "ChainReaction", "", useCardsSuffix: false, singular: "Chain Reaction", plural: "copies of Chain Reaction"));
            IEnumerator coroutine2 = base.PlayCardsFromLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ChainReaction", "", useCardsSuffix: false, singular: "Chain Reaction", plural: "copies of Chain Reaction"));
            //Move 1 copy of Unstable Isotope from the villain trash to the villain deck. 
            MoveCardDestination villianDeck = new MoveCardDestination(base.TurnTaker.Deck);
            IEnumerator coroutine3 = base.GameController.SelectCardFromLocationAndMoveIt(base.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "UnstableIsotope"), villianDeck.ToEnumerable<MoveCardDestination>(), cardSource: base.GetCardSource());
            //Shuffle the villain deck.
            IEnumerator coroutine4 = base.ShuffleDeck(this.DecisionMaker, base.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
                yield return base.GameController.StartCoroutine(coroutine3);
                yield return base.GameController.StartCoroutine(coroutine4);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
                base.GameController.ExhaustCoroutine(coroutine3);
                base.GameController.ExhaustCoroutine(coroutine4);
            }
            //{Gray} deals himself 2 energy damage.
            coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Energy, cardSource: base.GetCardSource());
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