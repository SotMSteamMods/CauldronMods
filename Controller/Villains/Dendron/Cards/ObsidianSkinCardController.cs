using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class ObsidianSkinCardController : CardController
    {
        //==============================================================
        // Reduce damage dealt to {Dendron} by 1.
        //==============================================================

        public static readonly string Identifier = "ObsidianSkin";

        private const int DamageAmountToReduce = 1;

        public ObsidianSkinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger(c => c == this.Card.Owner.CharacterCard, DamageAmountToReduce);

            base.AddTriggers();
        }

    }
}