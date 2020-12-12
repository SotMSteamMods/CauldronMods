using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class DermalAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // Reduce damage dealt to that hero by 1.
        //==============================================================

        public static string Identifier = "DermalAug";

        private const int DamageReduction = 1;

        public DermalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger(c => c == base.GetCardThisCardIsNextTo(), DamageReduction);
        }
    }
}