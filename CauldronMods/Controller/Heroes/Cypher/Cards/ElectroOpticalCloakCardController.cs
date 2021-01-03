using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class ElectroOpticalCloakCardController : CypherBaseCardController
    {
        //==============================================================
        // Augmented heroes are immune to damage.
        // At the start of your turn, destroy this card.
        //==============================================================

        public static string Identifier = "ElectroOpticalCloak";

        public ElectroOpticalCloakCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringAugmentedHeroes();
        }

        public override void AddTriggers()
        {
            // Augmented heroes are immune to damage.
            base.AddImmuneToDamageTrigger(dealDamage => GameController.IsCardVisibleToCardSource(dealDamage.Target, GetCardSource()) && IsAugmentedHeroCharacterCard(dealDamage.Target));

            // At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);

            base.AddTriggers();
        }
    }
}