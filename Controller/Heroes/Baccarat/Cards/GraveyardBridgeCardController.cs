namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class GraveyardBridgeCardController : CardController
    {
        #region Constructors

        public GraveyardBridgeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //You may shuffle a card from your trash into your deck. If you do, put a card with the same name from your trash into play.
            //Shuffle all copies of that card from your trash into your deck.

        }

        #endregion Methods
    }
}