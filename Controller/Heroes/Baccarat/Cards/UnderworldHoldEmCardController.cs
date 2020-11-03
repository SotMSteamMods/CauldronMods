namespace Cauldron.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;
    using Handelabra.Sentinels.Engine.Controller;

    public class UnderworldHoldEmCardController : CardController
    {
        #region Constructors

        public UnderworldHoldEmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //One player may draw a card.
            yield break;
        }

        #endregion Methods
    }
}