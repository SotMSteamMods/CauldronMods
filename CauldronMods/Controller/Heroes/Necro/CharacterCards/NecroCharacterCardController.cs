using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;

namespace Cauldron.Necro
{
    public class NecroCharacterCardController : HeroCharacterCardController
    {
        public NecroCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //PowerNumerals Required on powers
            int op1Targets = GetPowerNumeral(0, 1);
            int op1Damage = GetPowerNumeral(1, 1);
            int op2Targets = GetPowerNumeral(2, 1);
            int op2Damage = GetPowerNumeral(3, 2);

            //Option 1: Deal 1 Target 1 Toxic damage
            var response1 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), op1Damage, DamageType.Toxic, op1Targets, false, op1Targets, cardSource: base.GetCardSource());
            var op1 = new Function(this.DecisionMaker, $"Deal {op1Targets} target {op1Damage} toxic damage", SelectionType.DealDamage, () => response1);

            //Option: 2 Daeal 1 Undead Target 2 Toxic damage (Hide if no Undead Targets)
            var response2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), op2Damage, DamageType.Toxic, op2Targets, false, op2Targets,
                additionalCriteria: (Card c) => IsUndead(c) && c.IsTarget && c.IsInPlayAndHasGameText,
                cardSource: base.GetCardSource());
            var op2 = new Function(this.DecisionMaker, $"Deal {op2Targets} Undead target {op2Damage} toxic damage", SelectionType.DealDamage, () => response2,
                onlyDisplayIfTrue: base.GameController.GetAllCards().Any(c => IsUndead(c) && c.IsTarget && c.IsInPlayAndHasGameText));

            //Execute
            var options = new Function[] { op1, op2 };
            var selectFunctionDecision = new SelectFunctionDecision(base.GameController, this.DecisionMaker, options, false, cardSource: base.GetCardSource());
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One hero may themselves deal the 2 toxic damage to draw 2 cards now.

                        //Select a Hero
                        List<SelectCardDecision> storedHero = new List<SelectCardDecision>();
                        var criteria = new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero", false);
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DealDamageSelf, criteria, storedHero, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectCard(storedHero))
                        {
                            //Ask hero if they want to deal damage to themselves
                            Card heroCard = GetSelectedCard(storedHero);
                            HeroTurnTakerController httc = base.FindHeroTurnTakerController(heroCard.Owner.ToHero());
                            List<DealDamageAction> storedResults = new List<DealDamageAction>();

                            coroutine = base.GameController.DealDamageToSelf(httc, (Card c) => c == heroCard, 2, DamageType.Toxic, false, storedResults, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            coroutine = base.DrawCards(httc, 2);
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
                case 1:
                    {
                        //One hero may discard up to 3 cards, then regain 2 HP for each card discarded.

                        //Select hero to discard cards
                        List<SelectTurnTakerDecision> storedResultsTurnTaker = new List<SelectTurnTakerDecision>();
                        List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
                        IEnumerator coroutine = base.GameController.SelectHeroToDiscardCards(this.DecisionMaker, 0, 3,
                            storedResultsTurnTaker: storedResultsTurnTaker,
                            storedResultsDiscard: storedResultsDiscard,
                            cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //then determine hero character card to gain Hp.  This may not work with Obliveon reward characters
                        int numCardsDiscarded = GetNumberOfCardsDiscarded(storedResultsDiscard);
                        if (numCardsDiscarded > 0)
                        {
                            var httc = FindHeroTurnTakerController(GetSelectedTurnTaker(storedResultsTurnTaker).ToHero());
                            if (httc.HasMultipleCharacterCards)
                            {
                                coroutine = base.GameController.SelectAndGainHP(httc, numCardsDiscarded * 2, false, c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && c.IsInLocation(httc.HeroTurnTaker.PlayArea), cardSource: base.GetCardSource());
                            }
                            else
                            {
                                coroutine = base.GameController.GainHP(httc.CharacterCard, numCardsDiscarded * 2, cardSource: base.GetCardSource());
                            }
                        }
                        else
                        {
                            //no cards were discarded, send message to inform player of result
                            coroutine = base.GameController.SendMessageAction("No cards were discarded, so no HP is gained.", Priority.Medium, base.GetCardSource());
                        }

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
                        //Select a hero target. Increase damage dealt by that target by 3 and increase damage dealt to that target by 2 until the start of your next turn.

                        //Select a hero target
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.IncreaseDamage, new LinqCardCriteria((Card c) => !c.IsIncapacitatedOrOutOfGame && c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, "hero target"), storedResults, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card selectedTarget = GetSelectedCard(storedResults);

                            //Until start of turn, increase damage dealt by that selected card by 3
                            IncreaseDamageStatusEffect increaseDamageDealtStatusEffect = new IncreaseDamageStatusEffect(3);
                            increaseDamageDealtStatusEffect.SourceCriteria.IsSpecificCard = selectedTarget;
                            increaseDamageDealtStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            increaseDamageDealtStatusEffect.UntilTargetLeavesPlay(selectedTarget);
                            IEnumerator coroutine2 = base.AddStatusEffect(increaseDamageDealtStatusEffect);

                            //Until start of turn, increase damage dealt to that selected card by 2
                            IncreaseDamageStatusEffect increaseDamageTakenStatusEffect = new IncreaseDamageStatusEffect(2);
                            increaseDamageTakenStatusEffect.TargetCriteria.IsSpecificCard = selectedTarget;
                            increaseDamageTakenStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            increaseDamageTakenStatusEffect.UntilTargetLeavesPlay(selectedTarget);
                            IEnumerator coroutine3 = base.AddStatusEffect(increaseDamageTakenStatusEffect);

                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

        private bool IsUndead(Card card)
        {
            return card != null && card.DoKeywordsContain(NecroCardController.UndeadKeyword);
        }
    }
}
