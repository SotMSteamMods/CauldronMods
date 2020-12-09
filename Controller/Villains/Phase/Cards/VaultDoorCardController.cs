using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Phase
{
    public class VaultDoorCardController : CardController
    {
        public VaultDoorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{Phase} is immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard);
            //When this card would be dealt 2 or less damage, prevent that damage.
            base.AddPreventDamageTrigger((DealDamageAction action) => action.Target == base.Card && action.Amount <= 2);
        }
    }
}