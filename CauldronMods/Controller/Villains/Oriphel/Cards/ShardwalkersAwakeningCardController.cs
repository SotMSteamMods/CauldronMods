using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class ShardwalkersAwakeningCardController : OriphelUtilityCardController
    {
        public ShardwalkersAwakeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => base.CharacterCard.Title == "Oriphel", () => "Oriphel is in play.", () => "Jade is in play.");
        }

        public override IEnumerator Play()
        {
            //"If {Oriphel} is in play, he deals each hero target 1 infernal and 1 projectile damage.",
            IEnumerator coroutine;
            if (oriphelIfInPlay != null)
            {
                coroutine = OriphelDealsDamage();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //"If Jade is in play, flip her villain character cards."
            if (jadeIfInPlay != null)
            {
                coroutine = GameController.FlipCard(FindCardController(jadeIfInPlay), cardSource: GetCardSource());
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

        private IEnumerator OriphelDealsDamage()
        {
            var damageDetails = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, oriphelIfInPlay), null, 1, DamageType.Infernal),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, oriphelIfInPlay), null, 1, DamageType.Projectile)

            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damageDetails, (Card c) => c.IsTarget && IsHero(c) && oriphelIfInPlay != null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}