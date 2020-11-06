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

        #endregion Methods
    }
}