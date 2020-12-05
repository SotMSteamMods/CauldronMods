using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class LethalForceCardController : CardController
    {
        //==============================================================
        // {Vector} deals the hero target with the second highest HP {H - 1} melee damage.
        // {Vector} deals the hero target with the lowest HP {H - 2} melee damage.
        //==============================================================

        public static readonly string Identifier = "LethalForce";

        public LethalForceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            int highestHpDamage = base.Game.H - 1;
            IEnumerator routine = base.DealDamageToHighestHP(base.CharacterCard, 2, c => c.IsHero && c.IsTarget, c => new int?(highestHpDamage), DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            int lowestHpDamage = base.Game.H - 2;
            routine = base.DealDamageToLowestHP(base.CharacterCard, 1, c => c.IsHero && c.IsTarget, c => new int?(lowestHpDamage), DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            yield break;
        }
    }
}