using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.PhaseVillain
{
    public class ReinforcedWallCardController : PhaseCardController
    {
        public ReinforcedWallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //{Phase} is immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard);
            //Other Obstacles that enter play are immune to damage while this card is in play.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => this.IsNewerObstacle(action.Target) && action.Target != base.Card);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible while it has more than 0 HP.
            return card == base.Card && base.Card.HitPoints > 0;
        }

        private bool IsNewerObstacle(Card c)
        {
            return base.IsObstacle(c) && c.PlayIndex > base.Card.PlayIndex;
        }
    }
}