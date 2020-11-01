namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class CheapTrickCardController : CardController
    {
        #region Constructors

        public CheapTrickCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Discard the top card of your deck.
            //Reveal cards from the top of your deck until you reveal a trick. Shuffle the other cards back into your deck and put the trick into play.

        }

        #endregion Methods
    }
}