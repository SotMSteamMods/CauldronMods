using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class ShadowWeaverCardController : CardController
    {
        //==============================================================
        // At the end of the environment turn, this card deals the hero with the
        // lowest HP {H - 2} toxic damage.
        // When this card is destroyed, it deals each target 1 psychic damage.
        //==============================================================

        public static string Identifier = "ShadowWeaver";

        private const int PsychicDamageToDeal = 1;

        public ShadowWeaverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            int damageToDeal = Game.H - 2;

            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse,
                TriggerType.DestroySelf, null, false);


            base.AddTriggers();
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            yield break;
        }
    }
}