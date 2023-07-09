using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class TheMonBeskmaHydricCardController : MenagerieCardController
    {
        public TheMonBeskmaHydricCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria(c => IsEnclosure(c), "enclosure"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the Enclosure with the highest HP 4 melee damage and the hero target with the highest HP 2 melee damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage });
            base.AddTriggers();

        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the Enclosure with the highest HP 4 melee damage...
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => base.IsEnclosure(c), (Card c) => 4, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and the hero target with the highest HP 2 melee damage.
            coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => 2, DamageType.Melee);
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