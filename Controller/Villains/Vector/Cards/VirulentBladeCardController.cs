﻿using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VirulentBladeCardController : CardController
    {
        //==============================================================
        // When this card enters play, {Vector} deals himself 2 irreducible melee damage.
        // At the end of the villain turn, {Vector} deals each hero target 2 toxic damage.
        //==============================================================

        public static readonly string Identifier = "VirulentBlade";

        private const int DamageToDealSelf = 2;
        private const int DamageToDeal = 2;

        public VirulentBladeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn, {Vector} deals each hero target 2 toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator routine = this.GameController.DealDamageToSelf(this.DecisionMaker, c => c == this.CharacterCard, DamageToDealSelf,
                DamageType.Melee, true, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator routine = base.DealDamage(this.CharacterCard, c => c.IsHero && c.IsTarget && c.IsInPlay, DamageToDeal, DamageType.Toxic);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}