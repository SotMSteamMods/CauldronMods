﻿using System;
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

            //Deals 1 hero 3 toxic damage.
            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int numTargets = base.GetPowerNumeral(1, 1);
            int amount = base.GetPowerNumeral(1, PowerDamageAmount);

            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                damageSource,
                amount,
                DamageType.Toxic,
                new int?(numTargets), 
                false,
                new int?(numTargets),
                additionalCriteria: ((Func<Card, bool>)(c => c.IsHeroCharacterCard)),
                addStatusEffect: new Func<DealDamageAction, IEnumerator>(this.OnHeroDamageResponse),
                cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator OnHeroDamageResponse(DealDamageAction dd)
        {
            //If that hero took damage this way, they may play a card now.
            if (dd != null && dd.OriginalTarget == dd.Target && dd.DidDealDamage)
            {
                Card targetHero = dd.Target;
                HeroTurnTakerController heroController = null;
                if (targetHero.Owner.IsHero)
                {
                    heroController = base.FindHeroTurnTakerController(targetHero.Owner.ToHero());
                }
                IEnumerator coroutine = base.SelectAndPlayCardFromHand(heroController,overrideName: targetHero.Title);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
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
