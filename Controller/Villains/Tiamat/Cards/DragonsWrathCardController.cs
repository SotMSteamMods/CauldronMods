using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class DragonsWrathCardController : CardController
    {
        #region Constructors

        public DragonsWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Increase damage dealt by heads targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card.DoKeywordsContain("head"), (DealDamageAction dd) => 1, false);
        }

        #endregion Methods
    }
}