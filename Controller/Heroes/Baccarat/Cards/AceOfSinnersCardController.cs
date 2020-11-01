namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class AceOfSinnersCardController : CardController
    {
        #region Constructors

        public AceOfSinnersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Increase damage dealt by hero targets by 1.
            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck or this card is destroyed.

        }

        #endregion Methods
    }
}