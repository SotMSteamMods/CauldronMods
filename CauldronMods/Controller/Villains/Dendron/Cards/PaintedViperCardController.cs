using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class PaintedViperCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, this card deals
        // the hero target with the lowest HP {H - 2} toxic damage.
        //==============================================================

        public static readonly string Identifier = "PaintedViper";

        public PaintedViperCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            //  At the end of the villain turn, this card deals the hero target with the lowest HP {H - 2} toxic damage.
            int damageToDeal = Game.H - 2;
            IEnumerator dealDamageRoutine = DealDamageToLowestHP(Card, 1, (Card c) => IsHeroTarget(c), (Card c) => damageToDeal, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}