using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class ParadoxIntrusionCardController : CardController
    {
        #region Constructors

        public ParadoxIntrusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, base.Game.H, DamageType.Energy);
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsVillain, TargetType.SelectTarget, 2, DamageType.Energy, numberOfTargets: base.FindCardsWhere((Card c) => c.DoKeywordsContain("time vortex") && c.IsInTrash && c.IsEnvironment).Count<Card>());
        }

        private int NumberOfVorticiesInTrash()
        {
            //...the number of time vortex cards in the environment trash.
            return base.FindCardsWhere((Card c) => c.DoKeywordsContain("time vortex")).Count<Card>();
        }

        #endregion Methods
    }
}