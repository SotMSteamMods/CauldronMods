using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class PlatedGiantCardController : OblaskCraterUtilityCardController
    {
        /* 
         * Play this card next to a hero. The hero next to this card is immune to damage from enviroment targets 
         * other than this one. 
         * At the end of the environment turn, this card deals the hero next to it {H - 1} melee damage.
         */
        public PlatedGiantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, true, decisionSources);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt.IsEnvironment, PhaseChangeActionResponse, TriggerType.DealDamage);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            // At the end of the environment turn, this card deals the hero next to it {H - 1} melee damage.
            IEnumerator coroutine;

            coroutine = base.DealDamage(base.Card, base.GetCardThisCardIsNextTo(), base.H - 1, DamageType.Melee, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            // The hero next to this card is immune to damage from enviroment targets other than this one.
            IEnumerator coroutine;
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect;

            immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.SourceCriteria.IsTarget = true;
            immuneToDamageStatusEffect.SourceCriteria.IsEnvironment = true;
            immuneToDamageStatusEffect.SourceCriteria.IsNotSpecificCard = base.Card;
            immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = base.GetCardThisCardIsNextTo();

            coroutine = base.AddStatusEffect(immuneToDamageStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
