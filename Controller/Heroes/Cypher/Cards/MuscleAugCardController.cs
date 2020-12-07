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

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger(dd => dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo()), 
                DamageIncrease);
        }
    }
}