using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public static readonly string Identifier = "ShadowWeaver";

        private const int PsychicDamageToDeal = 1;

        public ShadowWeaverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, this card deals the hero with the lowest HP {H - 2} toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse,
                TriggerType.DealDamage);

            // When this card is destroyed, it deals each target 1 psychic damage.
            base.AddWhenDestroyedTrigger(DestroyCardResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetsWithLowestHitPoints(1, 1,
                c => c.IsHero && !c.IsIncapacitatedOrOutOfGame, storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithLowestHpRoutine);
            }

            if (!storedResults.Any())
            {
                yield break;
            }

            // At the end of the environment turn, this card deals the hero with the lowest HP {H - 2} toxic damage.
            int damageToDeal = Game.H - 2;

            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, storedResults.First(), 
                damageToDeal, DamageType.Toxic);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            IEnumerator dealDamageRoutine
                = this.DealDamage(this.Card,
                    card => !card.Equals(this.Card) && card.IsTarget, PsychicDamageToDeal, DamageType.Psychic);

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