using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class PastTheStrangerCharacterCardController : TheStrangerBaseCharacterCardController
    {
        public PastTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //The next time {TheStranger} would deal himself damage, redirect it to another target.
            RedirectDamageStatusEffect effect = new RedirectDamageStatusEffect();
            effect.SourceCriteria.IsSpecificCard = base.Card;
            effect.TargetCriteria.IsSpecificCard = base.Card;
            effect.RedirectableTargets.IsTarget = true;
            effect.RedirectableTargets.IsNotSpecificCard = base.Card;
            effect.NumberOfUses = 1;
            IEnumerator coroutine5 = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine5);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine5);
            }
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may use a power now.
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //Destroy 1 ongoing card.
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && c.IsInPlay, "ongoing"), false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;

                    }
                case 2:
                    {
                        //The target with the lowest HP deals itself 1 irreducible toxic damage.
                        List<Card> storedResult = new List<Card>();
                        IEnumerator coroutine3 = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsTarget && c.IsInPlayAndNotUnderCard, storedResult, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        if (storedResult.Count() > 0)
                        {
                            coroutine3 = base.DealDamage(storedResult.FirstOrDefault(), storedResult.FirstOrDefault(), 1,DamageType.Toxic, isIrreducible: true, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

    }
}
