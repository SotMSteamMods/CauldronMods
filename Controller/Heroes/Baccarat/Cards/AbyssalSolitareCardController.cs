namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    using Handelabra.Sentinels.Engine.Controller;
    using Handelabra.Sentinels.Engine.Model;

    public class AbyssalSolitareCardController : CardController
    {
        #region Constructors

        public AbyssalSolitareCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.

        }

        #endregion Methods
    }
}