using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfTheTwistedShadowCardController : RuneCardController
    {
        #region Constructors

        public MarkOfTheTwistedShadowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && !c.IsIncapacitatedOrOutOfGame, "hero targets", false, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //Increase damage dealt by that target by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card == base.GetCardThisCardIsNextTo(true), (DealDamageAction dd) => 1);

        }
        #endregion Methods
    }
}