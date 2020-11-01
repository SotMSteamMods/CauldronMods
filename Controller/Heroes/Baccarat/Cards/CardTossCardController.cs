namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class CardTossCardController : CardController
    {
        #region Constructors

        public CardTossCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //{Baccarat} deals 1 target 1 projectile damage.
            //You may play a card.

        }

        #endregion Methods
    }
}