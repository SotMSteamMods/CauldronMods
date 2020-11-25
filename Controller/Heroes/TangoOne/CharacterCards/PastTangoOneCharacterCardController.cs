using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PastTangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageToDeal = 3;
        private const int Incapacitate1CardsToDraw = 2;
        private const int Incapacitate2CardsToPlay = 2;
        private const int Incapacitate3CardsToDestroy = 1;

        public PastTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Select a target, at the start of your next turn,
            // {TangoOne} deals that target 3 projectile damage.
            //==============================================================

            /*
            IEnumerable<Card> targets = FindCardsWhere(card => card.IsTarget && card.IsInPlay);

            //base.GameController.selectt

            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            IEnumerator selectTargetRoutine
                = base.GameController.SelectTargetAndStoreResults(this.HeroTurnTakerController, targets, storedResults, false,
                    false, null, this.Card, card => 3, DamageType.Projectile, false, 
                    null, null, null, null, null,
                    SelectionType.SelectTargetNoDamage, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectTargetRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectTargetRoutine);
            }
            */

            /*
            int powerNumeral = GetPowerNumeral(0, 1);
            int[] powerNumerals = new int[1]
            {
                powerNumeral
            };
            OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(
                base.CardWithoutReplacements, "CounterDamageResponse",
                "Whenever a target deals damage to " + base.Card.Title + ", he may deal that target 1 melee damage.",
                new TriggerType[1] {TriggerType.DealDamage}, base.TurnTaker, base.Card, powerNumerals)
            {
                SourceCriteria = {IsTarget = true},
                TargetCriteria = {IsSpecificCard = base.Card},
                DamageAmountCriteria = {GreaterThan = 0}
            };

            onDealDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            onDealDamageStatusEffect.UntilTargetLeavesPlay(base.Card);
            onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.After;
            onDealDamageStatusEffect.DoesDealDamage = true;

            IEnumerator coroutine = AddStatusEffect(onDealDamageStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            */
            //OnDrawCardStatusEffect r = new OnDrawCardStatusEffect(this.Card, nameof(DrawTest), "dsdsd", new []{TriggerType.DrawCard}, this.HeroTurnTaker, false, this.Card);


            
            OnPhaseChangeStatusEffect r = new OnPhaseChangeStatusEffect(this.CardWithoutReplacements,
                nameof(CallTest), "test method", new[]
                {
                    TriggerType.PhaseChange
                }, this.Card);
            
            //r.UntilStartOfNextTurn(base.HeroTurnTaker);
            //r.UntilCardLeavesPlay(base.Card);
            //r.DoesDealDamage = false;
            //r.BeforeOrAfter = BeforeOrAfter.After;


            IEnumerator fff = base.GameController.AddStatusEffect(r, true, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(fff);
            }
            else
            {
                base.GameController.ExhaustCoroutine(fff);
            }
            
        }

        public IEnumerator DrawTest(DrawCardAction dca, TurnTaker hero, StatusEffect effect)
        {
            int i = 0;

            yield break;
        }

        public IEnumerator CallTest(PhaseChangeAction pca, TurnTaker hero, StatusEffect effect)
        {

            int i = 0;

            yield break;
        }

        public IEnumerator CounterDamageResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            int? num = null;
            if (powerNumerals != null)
            {
                num = powerNumerals.ElementAtOrDefault(0);
            }
            if (!num.HasValue)
            {
                num = 1;
            }
            if (dd.DamageSource.IsCard)
            {
                Card source = base.Card;
                if (hero != null && hero.IsHero)
                {
                    source = hero.CharacterCard;
                }
                IEnumerator coroutine = DealDamage(source, dd.DamageSource.Card, num.Value, DamageType.Melee, isIrreducible: false, optional: true, isCounterDamage: true, null, null, null, ignoreBattleZone: false, GetCardSource(effect));
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


        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may draw 2 cards.
                    //==============================================================

                    


                    break;

                case 1:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may play 2 cards.
                    //==============================================================



                    break;

                case 2:

                    //==============================================================
                    // Destroy an environment card.
                    //==============================================================

                    IEnumerator destroyRoutine 
                        = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, 
                            new LinqCardCriteria(c => c.IsEnvironment, "environment"), Incapacitate3CardsToDestroy, optional: false, 0, null, null, null, ignoreBattleZone: false, null, null, null, GetCardSource());
                    
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(destroyRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(destroyRoutine);
                    }

                    break;
            }
        }
    }
}
