using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class LasherAugCardController : AugCardController
    {
        public LasherAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            }
        }

        public override ITrigger[] AddRegularTriggers()
        {
            return new ITrigger[] {
                //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage...
                base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, Game.H, DamageType.Projectile),
                //...and destroys {H - 2} hero ongoing and/or equipment cards.
                base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyHeroOngoingOrEquipmentCardsResponse, TriggerType.DestroyCard)
            };
        }

        private IEnumerator DestroyHeroOngoingOrEquipmentCardsResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), Game.H - 2, cardSource: base.GetCardSource());
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

        public override ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //Absorb: at the start of the villain turn, destroy 1 hero ongoing or equipment card.
            return new ITrigger[] { base.AddReduceDamageTrigger((Card c) => c == cardThisIsUnder, 1) };
        }
    }
}