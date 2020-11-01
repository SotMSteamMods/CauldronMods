namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class BringDownTheHouseCardController : CardController
    {
        #region Constructors

        public BringDownTheHouseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.

        }

        #endregion Methods
    }
}