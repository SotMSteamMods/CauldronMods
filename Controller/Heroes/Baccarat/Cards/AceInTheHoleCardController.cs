namespace Cauldron.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;
    using Handelabra.Sentinels.Engine.Controller;

    public class AceInTheHoleCardController : CardController
    {
        #region Constructors

        public AceInTheHoleCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) 
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //You may play a card.
            //You may use {Baccarat}'s innate power twice during your phase this turn.
            yield break;
        }

        #endregion Methods
    }
}