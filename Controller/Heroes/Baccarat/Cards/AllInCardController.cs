namespace Cauldron.Baccarat
{
    using System;
    using System.Collections;
    
    using Handelabra.Sentinels.Engine.Model;
    using Handelabra.Sentinels.Engine.Controller;

    public class AllInCardController : CardController
    {
        #region Constructors

        public AllInCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Discard a card from your hand. If you do, {Baccarat} deals each non-hero target 1 infernal damage and 1 radiant damage.
            yield break;
        }

        #endregion Methods
    }
}