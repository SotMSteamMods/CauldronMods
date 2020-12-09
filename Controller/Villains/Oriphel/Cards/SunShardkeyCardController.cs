using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class SunShardkeyCardController : OriphelShardkeyCardController
    {
        public SunShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Shardkey transformation trigger
            base.AddTriggers();

            //"Whenever a hero uses a power, that hero deals themselves 2 psychic damage."
            AddTrigger((UsePowerAction p) => p.HeroUsingPower != null, SelfDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator SelfDamageResponse(UsePowerAction upa)
        {

            var storedHero = new List<Card> { };
            IEnumerator coroutine = FindCharacterCard(upa.HeroUsingPower.TurnTaker, SelectionType.DealDamageSelf, storedHero);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (storedHero.FirstOrDefault() != null)
            {
                var hero = storedHero.FirstOrDefault();
                coroutine = DealDamage(hero, hero, 2, DamageType.Psychic, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}