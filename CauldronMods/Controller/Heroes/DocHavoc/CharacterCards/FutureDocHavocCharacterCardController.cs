using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class FutureDocHavocCharacterCardController : DocHavocSubCharacterCardController
    {
        public FutureDocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "Heroes that gained HP this turn: " + HPGainersListString());
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{DocHavoc} deals 1 target 2 melee damage, or 1 hero that regained HP this turn uses a power."
            int numDamageTargets = GetPowerNumeral(0, 1);
            int numDamageAmount = GetPowerNumeral(1, 2);
            int numHeroPowers = GetPowerNumeral(2, 1);

            string targetPlural = numDamageTargets == 1 ? "target" : "targets";
            string heroUsesPlural = numHeroPowers == 1 ? "hero that gained HP uses" : "heroes that gained HP use";
            var functions = new List<Function>
            {
                new Function(DecisionMaker, $"Deal {numDamageTargets} {targetPlural} {numDamageAmount} melee damage", SelectionType.DealDamage, () => DealPowerDamage(numDamageTargets, numDamageAmount), forcedActionMessage: "No heroes have gained HP this turn."),
                new Function(DecisionMaker, $"{numHeroPowers} {heroUsesPlural} a power", SelectionType.UsePower, () => GrantPowersToHPGainers(numHeroPowers), HeroesThatGainedHP().Count() > 0)
            };
            var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator DealPowerDamage(int numTargets, int numDamage)
        {
            return GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numDamage, DamageType.Melee, numTargets, false, numTargets, cardSource: GetCardSource());
        }

        private IEnumerator GrantPowersToHPGainers(int numHeroes)
        {
            var selectHeroCards = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => HeroesThatGainedHP().Contains(c), SelectionType.UsePower, numHeroes, false, numHeroes, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(selectHeroCards, GrantSpecificHeroPower, cardSource: GetCardSource());
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

        private IEnumerator GrantSpecificHeroPower(SelectCardDecision scd)
        {
            var heroCC = FindCardController(scd?.SelectedCard);
            if(heroCC == null)
            {
                yield break;
            }
            IEnumerator coroutine = SelectAndUsePower(heroCC, optional: false);
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

        private List<Card> HeroesThatGainedHP()
        {
            return Journal.GainHPEntriesThisTurn().Where((GainHPJournalEntry j) => j.TargetCard.IsTarget && IsHeroCharacterCard(j.TargetCard) && GameController.IsCardVisibleToCardSource(j.TargetCard, GetCardSource())).Select(j => j.TargetCard).Distinct().ToList();
        }

        private string HPGainersListString()
        {
            var heroes = HeroesThatGainedHP();
            if (heroes.Any())
            {
                return String.Join(", ", HeroesThatGainedHP().Select((Card c) => c.Title).ToArray());
            }
            return "None";
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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

                case 1:
                    {
                        //"Destroy 1 ongoing card.",
                        coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), false, cardSource: GetCardSource());
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

                case 2:
                    {
                        //"The next time a hero target regains HP, increase the amount by 2."
                        var effect = new IncreaseGainHPStatusEffect(2);
                        effect.TargetCriteria.IsHero = true;
                        effect.TargetCriteria.IsTarget = true;
                        effect.CardSource = this.Card;
                        effect.NumberOfUses = 1;

                        coroutine = AddStatusEffect(effect);
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
            yield break;
        }
    }
}
