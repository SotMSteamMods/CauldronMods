namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class IFoldCardController : CardController
    {
        #region Constructors

        public IFoldCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Discard your hand and draw 3 cards.

        }

        #endregion Methods
    }
}