using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Quicksilver
{
    public class QuicksilverSubCharacterCardController : HeroCharacterCardController
    {
        public QuicksilverSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SetCardProperty(base.GeneratePerTargetKey(ComboSelfDamage, base.CharacterCard), false);
        }

        private const string ComboSelfDamage = "ComboSelfDamage";
    }
}