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
            List<Card> lowestHpTarget = new List<Card>();
            IEnumerator routine = base.GameController.FindTargetWithLowestHitPoints(1, c => c.IsHero, lowestHpTarget, cardSource: base.GetCardSource());

            List<Card> highestHpTarget = new List<Card>();
            IEnumerator routine2 = base.GameController.FindTargetWithHighestHitPoints(2, c => c.IsHero, highestHpTarget, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
                yield return base.GameController.StartCoroutine(routine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
                base.GameController.ExhaustCoroutine(routine2);
            }

            if (highestHpTarget.Any())
            {
                int highestHpDamage = base.Game.H - 1;
                routine = base.DealDamage(this.CharacterCard, c => c == lowestHpTarget.First(), highestHpDamage, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            if (!lowestHpTarget.Any())
            {
                yield break;
            }

            int lowestHpDamage = base.Game.H - 2;
            routine = base.DealDamage(this.CharacterCard, c => c == lowestHpTarget.First(), lowestHpDamage, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}