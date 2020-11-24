using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class ForestOfNeedlesCardController : CardController
    {
        public ForestOfNeedlesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //{Quicksilver} may deal 6 melee damage to a target with more than 8HP, or 3 melee damage to a target with 8 or fewer HP.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), (Card c) => this.DynamicDamage(c), DamageType.Melee, () => 1, false, 1, cardSource: base.GetCardSource());
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

        private int DynamicDamage(Card c)
        {
            int amount = 0;
            if (c.HitPoints > 8)
            {
                amount = 6;
            }
            else
            {
                amount = 3;
            }
            return amount;
        }
    }
}