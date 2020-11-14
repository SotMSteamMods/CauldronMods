using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class MirrorWraithCardController : CardController
    {
        //==============================================================
        // When this card enters play,
        // it gains the text, keywords, and max HP of the non-character
        // target with the lowest HP in play.
        // If there are no other non-character targets in play when
        // this card enters play, this card deals each
        // target 2 sonic damage and is destroyed.
        //==============================================================

        public static string Identifier = "MirrorWraith";

        private const int DamageToDeal = 2;

        public MirrorWraithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetsWithLowestHitPoints(1, 1,
                c => c.IsTarget && c.IsInPlay && !c.IsCharacter && !c.Equals(this.Card), storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithLowestHpRoutine);
            }

            if (!storedResults.Any())
            {
                // No eligible targets were found, deal all targets 2 sonic damage
                IEnumerator dealDamageRoutine
                    = this.DealDamage(this.Card, card => card.IsTarget && !card.Equals(this.Card), DamageToDeal,
                        DamageType.Sonic);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageRoutine);
                }
            }
            else
            {
                // Gains the text, keywords, and max HP of found target
                Card cardToCopy = storedResults.First();
                
                IEnumerator setHpRoutine = base.GameController.SetHP(this.Card, cardToCopy.MaximumHitPoints.Value, this.GetCardSource());

                //base.TurnTakerController.

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(setHpRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(setHpRoutine);
                }

                int i = 0;
            }
        }

        private IEnumerator DealDamageResponse()
        {
            yield break;

        }
    }
}