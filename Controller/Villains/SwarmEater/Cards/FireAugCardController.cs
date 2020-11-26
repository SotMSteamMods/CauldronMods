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

        public override ITrigger[] AddRegularTriggers()
        {
            return new ITrigger[] {
                //At the end of the villain turn this card deals the hero target with the second highest HP {H - 1} fire damage... 
                base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, Game.H - 1, DamageType.Fire, highestLowestRanking: 2),
                //...and each player must discard a card.
                base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DiscardCardResponse, TriggerType.DiscardCard)
            };
        }

        private IEnumerator DiscardCardResponse(PhaseChangeAction action)
        {
            //...and each player must discard a card.
            IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, cardSource: base.GetCardSource());
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
            //Absorb: at the start of the villain turn, {H - 2} players must discard a card.
            return new ITrigger[] { base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.AbsorbDiscardResponse, TriggerType.DiscardCard) };
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