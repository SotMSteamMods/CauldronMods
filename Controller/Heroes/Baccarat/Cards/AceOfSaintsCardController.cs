namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class AceOfSaintsCardController : CardController
    {
        #region Constructors

        public AceOfSaintsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Reduce damage dealt to hero targets by 1.
            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck or this card is destroyed.

        }

        #endregion Methods
    }
}