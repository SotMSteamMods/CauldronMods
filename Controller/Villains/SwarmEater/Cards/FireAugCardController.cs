using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class FireAugCardController : AugCardController
    {
        public FireAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP(2);
            }
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the second highest HP {H - 1} fire damage and each player must discard a card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageAndDiscardCardResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DiscardCard });

            //Absorb: at the start of the villain turn, {H - 2} players must discard a card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => base.Card.Location.IsUnderCard && tt == base.TurnTaker, this.AbsorbDiscardResponse, TriggerType.DiscardCard);
        }

        private IEnumerator DealDamageAndDiscardCardResponse(PhaseChangeAction action)
        {
            //this card deals the hero target with the second highest HP {H - 1} fire damage... 
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 2, (Card c) => c.IsHero && c.IsTarget, (Card c) => Game.H - 1, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...and each player must discard a card.
            coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, cardSource: base.GetCardSource());
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

        private IEnumerator AbsorbDiscardResponse(PhaseChangeAction action)
        {
            //...{H - 2} players must discard a card.
            IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, requiredNumberOfHeroes: Game.H - 2, cardSource: base.GetCardSource());
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