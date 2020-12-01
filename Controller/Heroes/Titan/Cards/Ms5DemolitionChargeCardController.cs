using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class Ms5DemolitionChargeCardController : CardController
    {
        public Ms5DemolitionChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"));
        }

        public override IEnumerator Play()
        {
            //Destroy all environment cards.
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            IEnumerator coroutine = base.DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment), this.DealDamageResponse);
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

        private IEnumerator DealDamageResponse(int count)
        {
            //...{Titan} deals himself 1 fire damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Fire, cardSource: base.GetCardSource());
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