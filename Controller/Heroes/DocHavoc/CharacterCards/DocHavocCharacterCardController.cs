using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocHavocCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 3;

        public DocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Adrenaline: Deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            //==============================================================

            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int powerNumeral = base.GetPowerNumeral(0, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource, powerNumeral,
                DamageType.Toxic, new int?(1), false, new int?(1),
                additionalCriteria: ((Func<Card, bool>)(c => c.IsHero)),
                storedResultsDamage: storedDamageResults, cardSource: base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!this.DidIntendedTargetTakeDamage((IEnumerable<DealDamageAction>)storedDamageResults,
                storedDamageResults.First().Target))
            {
                yield break;
            }

            // Intended damage taken, allow hero to draw a card
            IEnumerator drawCardRoutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator routine = null;
            switch (index)
            {
                case 0:

                    routine = GetIncapacitateOption1();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;

                case 1:

                    routine = DoIncapacitateOption2();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                    break;

                case 2:

                    routine = DoIncapacitateOption3();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;
            }
        }

        private IEnumerator GetIncapacitateOption1()
        {
            //==============================================================
            // Up to 3 hero targets regain 1 HP each.
            //==============================================================

            return this.GameController.SelectAndGainHP(this.DecisionMaker, 1, additionalCriteria: ((Func<Card, bool>)(c => c.IsHero)),
                numberOfTargets: 3, requiredDecisions: new int?(0), cardSource: this.GetCardSource());
        }

        private IEnumerator DoIncapacitateOption2()
        {
            //==============================================================
            // One player may draw a card now.
            //==============================================================

            return this.GameController.SelectHeroToDrawCard(this.DecisionMaker);
        }

        private IEnumerator DoIncapacitateOption3()
        {
            //==============================================================
            // Environment cards cannot deal damage until the start of your next turn..
            //==============================================================

            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.IsPreventEffect = true;
            cannotDealDamageStatusEffect.SourceCriteria.IsEnvironment = true;
            cannotDealDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);

            return this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, this.GetCardSource());
        }
    }
}
