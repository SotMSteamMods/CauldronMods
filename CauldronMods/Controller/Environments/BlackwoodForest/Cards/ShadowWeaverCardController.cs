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
            base.SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, this card deals the hero with the lowest HP {H - 2} toxic damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                    TargetType.LowestHP, H - 2, DamageType.Toxic,
                    highestLowestRanking: 1,
                    numberOfTargets: 1);

            // When this card is destroyed, it deals each target 1 psychic damage.
            base.AddWhenDestroyedTrigger(DestroyCardResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            var coroutine = this.DealDamage(this.Card, card => card.IsTarget, PsychicDamageToDeal, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}