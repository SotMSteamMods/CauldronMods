using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class MuscleAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // Increase damage dealt by that hero by 1.
        //==============================================================

        public static string Identifier = "MuscleAug";

        private const int DamageIncrease = 1;

        public MuscleAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(DamageIncrease)
            {
                SourceCriteria = {IsSpecificCard = base.GetCardThisCardIsNextTo()}, 
                CardSource = this.Card
            };
            idse.UntilCardLeavesPlay(this.Card);

            IEnumerator routine = base.AddStatusEffect(idse, true);
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