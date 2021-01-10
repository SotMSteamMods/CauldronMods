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
            base.SpecialStringMaker.ShowSpecialString(() => base.DeckIconList());
            base.SpecialStringMaker.ShowSpecialString(() => base.ThisCardsIcon());
        }

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
            List<Card> storedPlayResults = new List<Card>();
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.Identifier == "ClockworkRevenant"), 1, storedPlayResults: storedPlayResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If no card was put into play this way, this card deals each non-villain target 3 lightning damage.
            if (!storedPlayResults.Any())
            {
                coroutine = base.DealDamage(this.Card, (Card c) => !base.IsVillain(c), 3, DamageType.Lightning);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
