using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class DoktorVonFaustCardController : MythosUtilityCardController
    {
        public DoktorVonFaustCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.Identifier == "ClockworkRevenant", "Clockwork Revenant"));
        }

        private const string ClockworkRevenantIdentifier = "ClockworkRevenant";

        public override void AddTriggers()
        {
            //At the end of the villain turn, search the villain deck for a Clockwork Revenant and put it into play. Shuffle the villain deck. If no card was put into play this way, this card deals each non-villain target 3 lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[] { TriggerType.PutIntoPlay, TriggerType.DealDamage });

            //{MythosClue} Reduce damage dealt to this card by 2.
            base.AddReduceDamageTrigger((Card c) => c == this.Card && base.IsTopCardMatching(MythosClueDeckIdentifier), 2);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction action)
        {
            //...search the villain deck for a Clockwork Revenant and put it into play. Shuffle the villain deck. 
            List<bool> storedPlayResults = new List<bool>();
            IEnumerator coroutine = PlayRevenentFromDeckThenShuffle(storedPlayResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If no card was put into play this way, this card deals each non-villain target 3 lightning damage.
            if (storedPlayResults.Any() && storedPlayResults.First() == true)
            {
                yield break;
            }
            coroutine = base.DealDamage(this.Card, (Card c) => !base.IsVillainTarget(c), 3, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            
            yield break;
        }

        private IEnumerator PlayRevenentFromDeckThenShuffle(List<bool> storedPlay)
        {
          
            //When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocation(base.TurnTaker.Deck, ClockworkRevenantIdentifier, isPutIntoPlay: true, wasCardPlayed: storedPlay, shuffleAfterwardsIfDeck: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: GetCardSource());
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
