using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfQuickeningCardController : RuneCardController
    {
        #region Constructors

        public MarkOfQuickeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero", true, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}