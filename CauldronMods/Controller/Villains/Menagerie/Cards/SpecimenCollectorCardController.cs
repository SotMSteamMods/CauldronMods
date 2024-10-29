using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class SpecimenCollectorCardController : MenagerieCardController
    {
        public SpecimenCollectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithLowestHP(2);
            base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => base.FindLocationsWhere((Location loc) => loc.IsUnderCard && loc.HasCards));

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the non-villain target with the second lowest HP {H} projectile damage. Then, place the top card of the villain deck face down beneath the Enclosure with the fewest cards beneath it.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageAndEncloseResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.MoveCard });
            base.AddTriggers();

        }

        private IEnumerator DealDamageAndEncloseResponse(PhaseChangeAction action)
        {
            //...this card deals the non-villain target with the second lowest HP {H} projectile damage. 
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 2, (Card c) => !base.IsVillainTarget(c), (Card c) => Game.H, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, place the top card of the villain deck face down beneath the Enclosure with the fewest cards beneath it.
            IEnumerable<Card> enclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsEnclosure(c) && c.IsInPlayAndHasGameText));
            if (enclosures.Any())
            {
                Card fewestEnclosed = enclosures.FirstOrDefault();
                foreach (Card enclosure in enclosures)
                {
                    if (fewestEnclosed.UnderLocation.NumberOfCards > enclosure.UnderLocation.NumberOfCards)
                    {
                        fewestEnclosed = enclosure;
                    }
                }
                coroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, fewestEnclosed.UnderLocation, cardSource: base.GetCardSource());
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
    }
}
