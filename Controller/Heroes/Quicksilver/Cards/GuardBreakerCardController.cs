using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class GuardBreakerCardController : CardController
    {
        public GuardBreakerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy a target with 3 or fewer HP, or deal 1 target 3 irreducible melee damage.
            List<Function> functions = new List<Function> {
                //Destroy a target with 3 or fewer HP...
                new Function(base.HeroTurnTakerController, "Destroy a target with 3 or fewer HP", SelectionType.DestroyCard, () => base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController,new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 3, "target with 3 or fewer HP"), false,cardSource: base.GetCardSource())),
                //...deal 1 target 3 irreducible melee damage.
                new Function(base.HeroTurnTakerController, "deal 1 target 3 irreducible melee damage", SelectionType.DealDamage, () => base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Melee, 1, false, new int?(1), true, cardSource: base.GetCardSource()))
            };
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, functions);
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