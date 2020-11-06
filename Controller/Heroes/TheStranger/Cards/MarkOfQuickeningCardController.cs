using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfQuickeningCardController : RuneCardController
    {
        #region Constructors

        public MarkOfQuickeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero", true, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //They may play an additional card during their play phase.
            base.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.PlayCard, 1);
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == base.GetCardThisCardIsNextTo(true).Owner.ToHero(); ;
        }
        #endregion Methods
    }
}