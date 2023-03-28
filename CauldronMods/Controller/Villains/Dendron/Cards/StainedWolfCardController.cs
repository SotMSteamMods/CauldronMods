using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class StainedWolfCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, this card deals
        // the hero target with the highest HP {H - 1} melee damage.
        //==============================================================

        public static readonly string Identifier = "StainedWolf";

        public StainedWolfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {

            //  At the end of the villain turn,  this card deals the hero target with the highest HP {H - 1} melee damage.
            int damageToDeal = Game.H - 1;
            IEnumerator dealDamageRoutine = DealDamageToHighestHP(Card, 1, (Card c) => IsHero(c), (Card c) => damageToDeal, DamageType.Melee);
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