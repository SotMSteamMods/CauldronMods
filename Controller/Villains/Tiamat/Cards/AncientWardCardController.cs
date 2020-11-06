using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class AncientWardCardController : CardController
    {
        #region Constructors

        public AncientWardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Reduce damage dealt to Heads by 1.
            base.AddReduceDamageTrigger((Card c) => c.DoKeywordsContain("head"), 1);
        }

        #endregion Methods
    }
}