namespace Cauldron.Controller.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;

    public class AfterlifeEuchreCardController : CardController
    {
        #region Constructors

        public AfterlifeEuchreCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Increase the next damage dealt by {Baccarat} by 1, or {Baccarat} deals 1 target 2 toxic damage.

        }

        #endregion Methods
    }
}