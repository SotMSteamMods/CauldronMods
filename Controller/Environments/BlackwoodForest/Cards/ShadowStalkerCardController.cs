using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class ShadowStalkerCardController : CardController
    {
        //==============================================================
        // At the end of the environment turn, destroy 1 ongoing card.
        // At the start of the environment turn this card deals each
        // hero target {H - 2} melee damage.
        // Whenever an environment card is destroyed, this card deals
        // the target with the highest HP 5 psychic damage
        //==============================================================

        public static string Identifier = "ShadowStalker";

        private const int PsychicDamageToDeal = 5;


        public ShadowStalkerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, destroy 1 ongoing card.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDestroyOngoingResponse,
                TriggerType.DestroyCard, null, false);

            // At the start of the environment turn this card deals each hero target {H - 2} melee damage.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, 
                StartOfTurnDealDamageResponse, TriggerType.DealDamage);


            base.AddTriggers();
        }

        private IEnumerator EndOfTurnDestroyOngoingResponse(PhaseChangeAction pca)
        {
            // Destroy 1 ongoing card
            IEnumerator destroyCardRoutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker,
                new LinqCardCriteria(card => card.IsOngoing), 
                false, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }
        }

        private IEnumerator StartOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            // Deal each hero target {H - 2} melee damage
            int damageToDeal = Game.H - 2;

            IEnumerator dealDamageRoutine
                = this.DealDamage(this.Card,
                    card => !card.Equals(this.Card) && card.IsHero && !card.IsIncapacitatedOrOutOfGame,
                    damageToDeal, DamageType.Melee);

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