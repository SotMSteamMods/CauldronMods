using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfBreakingCardController : RuneCardController
    {
        #region Constructors

        public MarkOfBreakingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsTarget, "targets", false, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //Increase damage dealt to that target by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target == base.GetCardThisCardIsNextTo(true), 1);
        }
    }
    #endregion Methods
}
