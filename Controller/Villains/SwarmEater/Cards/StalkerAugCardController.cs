using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class StalkerAugCardController : AugCardController
    {
        public StalkerAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithLowestHP();
            }
        }

        public override ITrigger[] AddRegularTriggers()
        {
            //At the end of the villain turn this card deals each hero target except the hero with the lowest HP {H - 2} irreducible energy damage.
            return new ITrigger[] { base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageExceptLowestResponse, TriggerType.DealDamage) };
        }

        public override ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //At the end of the villain turn, {SwarmEater} deals each other target 1 irreducible energy damage.
            return new ITrigger[] { base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, cardThisIsUnder, (Card c) => c != cardThisIsUnder, TargetType.All, 1, DamageType.Energy, true) };
        }

        private IEnumerator DealDamageExceptLowestResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.DealDamage(base.Card, (Card card) => card.IsHero, Game.H - 2, DamageType.Energy, damageSourceInfo: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsHero, "the hero target with the lowest HP")));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.FindActiveHeroTurnTakerControllers().Count<HeroTurnTakerController>() == 1)
            {
                coroutine = base.GameController.SendMessageAction(base.Card.Title + " does not deal damage because there is only one hero target.", Priority.Medium, base.GetCardSource(null), null, false);
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