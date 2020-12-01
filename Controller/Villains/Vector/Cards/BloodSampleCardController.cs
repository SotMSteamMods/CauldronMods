﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class BloodSampleCardController : VectorBaseCardController
    {
        //==============================================================
        // When this card enters play, {Vector} deals each hero target 1 toxic damage.
        //
        // At the start of the villain turn, if Supervirus is in play
        // and {Vector} was dealt {H x 2} or more damage last round,
        // you may put this card beneath Supervirus.
        //==============================================================

        public static readonly string Identifier = "BloodSample";

        private const int DamageToDeal = 1;

        public BloodSampleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker,
                StartOfTurnResponse, TriggerType.MoveCard);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator damageRoutine = this.DealDamage(this.CharacterCard, c => c.IsHero && c.IsInPlay, 
                DamageToDeal, DamageType.Toxic);
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            if (!IsSuperVirusInPlay() || !WasVectorDealtSufficientDamage())
            {
                yield break;
            }

            // Ask if players want to put this card underneath Super Virus
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.MoveCard, this.Card, null, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!base.DidPlayerAnswerYes(storedYesNoResults))
            {
                yield break;
            }

            routine = this.GameController.MoveCard(this.DecisionMaker, this.Card, 
                GetSuperVirusCard().UnderLocation, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // They chose to move the card under Super Virus, check for flip condition
            if (ShouldVectorFlip())
            {
                // TODO: Flip Vector
            }
        }

        private bool WasVectorDealtSufficientDamage()
        {
            int damageThreshold = Game.H * 2;

            int damageLastRound = base.GameController.Game.Journal.DealDamageEntries().Where(j => j.TargetCard == this.CharacterCard 
                && j.Round == Game.Round - 1).Select(j => j.Amount).Sum();

            return damageLastRound >= damageThreshold;
        }
    }
}