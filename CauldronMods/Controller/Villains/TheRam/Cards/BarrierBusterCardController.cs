using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class BarrierBusterCardController : TheRamUtilityCardController
    {
        public BarrierBusterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => c.IsInPlay && c.IsEnvironment, "environment"));
        }

        public override IEnumerator Play()
        {
            //"Destroy all environment cards. {TheRam} deals each non-villain target X melee damage, where X = 3 plus the number of cards destroyed this way."
            IEnumerator coroutine = DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(
                                                    DecisionMaker, 
                                                    new LinqCardCriteria((Card c) => c.IsEnvironment), 
                                                    DamageIfRamInPlay);
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

        private IEnumerator DamageIfRamInPlay(int cardsDestroyed)
        {
            IEnumerator coroutine;
            if(RamIfInPlay != null)
            {
                coroutine = DealDamage(GetRam, (Card c) => !IsVillainTarget(c), cardsDestroyed + 3, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = MessageNoRamToAct(GetCardSource(), "deal damage");
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