using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfBindingCardController : RuneCardController
    {
        #region Constructors

        public MarkOfBindingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => !c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, "non-hero targets", false, false, null, null, false))
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //Reduce damage dealt by that target by 1.
            base.AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card == base.GetCardThisCardIsNextTo(true), new int?(1), null);

        }
        #endregion Methods
    }
}