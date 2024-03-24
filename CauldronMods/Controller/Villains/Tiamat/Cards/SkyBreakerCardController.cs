using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class SkyBreakerCardController : CardController
    {
        public SkyBreakerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHighestHP(ranking: 1, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "head", false));

        }

        public override IEnumerator Play()
        {
            //The Head with the highest HP deals each hero target {H + 2} infernal damage.
            TargetInfo targetInfo = new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsTarget, "the head with the highest HP"));
            IEnumerator coroutine = base.DealDamage(null, (Card c) => IsHeroTarget(c), base.H + 2, DamageType.Infernal, damageSourceInfo: targetInfo);
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