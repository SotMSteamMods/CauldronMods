using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class AcidBreathCardController : CardController
    {

        public AcidBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHighestHP(ranking: 1, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "head", false));

        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            //Destroy 2 hero ongoing cards and 2 hero equipment cards.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && IsHero(c)), new int?(2), storedResultsAction: storedResultsAction, cardSource: this.GetCardSource());
            IEnumerator coroutine2 = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c) && IsHero(c)), new int?(2), storedResultsAction: storedResultsAction, cardSource: this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            //The Head with the highest HP deals {H} toxic damage to each hero who did not destroy a card this way.
            List<TurnTaker> heroesWithDestroyed = new List<TurnTaker>();
            if (storedResultsAction.Count<DestroyCardAction>() > 0)
            {
                foreach(DestroyCardAction destroy in storedResultsAction)
                {
                    heroesWithDestroyed.Add(destroy.CardToDestroy.TurnTaker);
                }
            }
            coroutine = base.DealDamage(null, (Card c) =>  IsHeroCharacterCard(c) && !heroesWithDestroyed.Any(tt => c.Owner == tt), base.H, DamageType.Toxic, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "the head with the highest HP")));
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