using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfTheTwistedShadowCardController : RuneCardController
    {
        #region Constructors

        public MarkOfTheTwistedShadowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && !c.IsIncapacitatedOrOutOfGame, "hero targets", false, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods
       
        #endregion Methods
    }
}