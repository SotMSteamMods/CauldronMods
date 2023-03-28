using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfBreakingCardController : RuneCardController
    {
        #region Constructors

        public MarkOfBreakingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties
        public override LinqCardCriteria NextToCardCriteria => new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "targets", useCardsSuffix: false);
        #endregion

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
