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

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage...
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, Game.H, DamageType.Projectile);
            //...and destroys {H - 2} hero ongoing and/or equipment cards.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyTwoHeroOngoingOrEquipmentCardsResponse, TriggerType.DestroyCard);
        }

        private IEnumerator DestroyTwoHeroOngoingOrEquipmentCardsResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), 2, cardSource: base.GetCardSource());
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

        public override IEnumerator ActivateAbsorb(Card cardThisIsUnder)
        {
            if (cardThisIsUnder.Identifier == "AbsorbedNanites")
            {
                cardThisIsUnder = base.CharacterCard;
            }
            //Absorb: at the start of the villain turn, destroy 1 hero ongoing or equipment card.
            base.AddReduceDamageTrigger((Card c) => c == cardThisIsUnder, 1);
            yield break;
        }
    }
}