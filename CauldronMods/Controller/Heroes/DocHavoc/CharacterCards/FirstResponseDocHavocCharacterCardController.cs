using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class FirstResponseDocHavocCharacterCardController : DocHavocSubCharacterCardController
    {
        private const int PowerNumberOfTargets = 1;
        private const int PowerDamageToDeal = 1;
        private const int Incapacitate1HpThreshold = 6;
        private const int Incapacitate1DamageReduction = 1;
        private const int Incapacitate3CardsToDestroy = 2;

        public FirstResponseDocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Draw a card. {DocHavoc} may deal 1 target 1 melee damage.
            //==============================================================

            IEnumerator routine = this.DrawCard(this.HeroTurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            int targetsNumeral = GetPowerNumeral(0, PowerNumberOfTargets);
            int damageNumeral = GetPowerNumeral(1, PowerDamageToDeal);
            DamageSource ds = new DamageSource(this.GameController, this.Card);
            IEnumerator routine2 = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, ds, damageNumeral,
                DamageType.Melee, targetsNumeral, false, 0, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine2);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Until the start of your next turn, reduce damage
                    // dealt to targets with fewer than 6 HP by 1
                    //==============================================================

                    List<Card> targets
                        = FindCardsWhere(card => card.IsTarget && card.IsInPlay
                            && card.HitPoints < Incapacitate1HpThreshold).ToList();
                    //This is not dynamic. It only affects those below 6 at time of execution
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(Incapacitate1DamageReduction);
                    rdse.TargetCriteria.IsOneOfTheseCards = targets;
                    rdse.UntilStartOfNextTurn(base.TurnTaker);

                    coroutine = base.AddStatusEffect(rdse, true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;

                case 1:

                    //==============================================================
                    // One player may take a one-shot from their trash and
                    // put it on top of their deck.
                    //==============================================================

                    coroutine = base.GameController.SelectHeroToMoveCardFromTrash(this.HeroTurnTakerController,
                        httc => httc.HeroTurnTaker.Deck,
                        cardCriteria: new LinqCardCriteria(c => c.IsInTrash && c.Location.IsHero && c.IsOneShot),
                        cardSource: GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;

                case 2:

                    //==============================================================
                    // Destroy 2 environment targets.
                    //==============================================================

                    coroutine = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController,
                        new LinqCardCriteria(c => c.IsEnvironmentTarget, "environment target"),
                        Incapacitate3CardsToDestroy,
                        requiredDecisions: 0, cardSource: GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;
            }
        }
    }
}
