using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    // "Each hero gains the following power:"
    // Power
    //  {Gargoyle} deals 1 target 2 toxic damage.
    //  {Gargoyle} may deal a second target 1 melee damage.
    public class MarkForExecutionCardController : GargoyleUtilityCardController
    {
        public MarkForExecutionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddAsPowerContributor();
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            Power[] powers = null;

            if (IsHeroCharacterCard(cardController.Card) == true && cardController.Card != base.CharacterCard && cardController.Card.IsInPlayAndHasGameText && cardController.Card.IsRealCard && !cardController.Card.IsIncapacitatedOrOutOfGame)
            {
                return new Power[]
                {
                    new Power(cardController.HeroTurnTakerController, cardController, "{Gargoyle} deals 1 target 2 toxic damage.{BR}{Gargoyle} may deal a second target 1 melee damage.", this.DealDamagePower(cardController.HeroTurnTakerController), cardController.Card.NumberOfPowers, null, base.GetCardSource())
                };
            }

            return powers;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = DealDamagePower(DecisionMaker);
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

        private IEnumerator DealDamagePower(HeroTurnTakerController hero)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            List<Card> selectedCards = new List<Card>();
            int toxicDamageTargets = base.GetPowerNumeral(0, 1);
            int toxicDamageAmount = base.GetPowerNumeral(1, 2);
            int meleeDamageAmount = base.GetPowerNumeral(2, 1);

            //{Gargoyle} deals 1 target 2 toxic damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(hero, new DamageSource(base.GameController, base.CharacterCard), toxicDamageAmount, DamageType.Toxic, toxicDamageTargets, false, toxicDamageTargets, storedResultsDecisions: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.Count() > 0)
            {
                selectedCards = storedResults.Select(scd => scd.SelectedCard).ToList();
            }

            //{Gargoyle} may deal a second target 1 melee damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(hero, new DamageSource(base.GameController, base.CharacterCard), meleeDamageAmount, DamageType.Melee, 1, false, 0, additionalCriteria: (card) => !selectedCards.Contains(card), cardSource: base.GetCardSource());
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
