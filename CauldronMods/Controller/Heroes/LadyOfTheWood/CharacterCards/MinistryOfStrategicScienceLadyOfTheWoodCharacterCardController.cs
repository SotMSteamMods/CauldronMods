using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.LadyOfTheWood
{
    public class MinistryOfStrategicScienceLadyOfTheWoodCharacterCardController : LadyOfTheWoodUtilityCharacterCardController
    {
        public MinistryOfStrategicScienceLadyOfTheWoodCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        { 
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Add 2 tokens to your element pool. 
            int tokensToAdd = GetPowerNumeral(0, 2);
            bool otherHeroUsingPower = false;

            if (TurnTaker.Identifier != LadyOfTheWoodIdentifier)
            {
                otherHeroUsingPower = true;
            }
            TokenPool elementPool = GetElementTokenPool();

            IEnumerator coroutine = base.GameController.AddTokensToPool(elementPool, tokensToAdd, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (otherHeroUsingPower)
            {
                OnDealDamageStatusEffect effect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(ChangeDamageTypeResponse), "When " + base.Card.Title + " would deal damage, you may change its type by spending an element token.", new TriggerType[]
                {
                TriggerType.ModifyTokens,
                TriggerType.ChangeDamageType
                }, base.TurnTaker, base.Card);
                effect.SourceCriteria.IsSpecificCard = base.Card;
                effect.UntilTargetLeavesPlay(base.Card);
                effect.BeforeOrAfter = BeforeOrAfter.Before;
                effect.CanEffectStack = true;

                if (GameController.StatusEffectManager.StatusEffectControllers.Any(sec => sec.StatusEffect is OnDealDamageStatusEffect removeToken && removeToken.MethodToExecute == nameof(ChangeDamageTypeResponse) && removeToken.Description == "When " + base.Card.Title + " would deal damage, you may change its type by spending an element token."))
                {
                    yield break;
                }
                coroutine = AddStatusEffect(effect);
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

        public IEnumerator ChangeDamageTypeResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            // You may change its type by spending a token

            TokenPool elementPool = GetElementTokenPool();

            if (elementPool is null || elementPool.CurrentValue == 0)
            {
                yield break;
            }

            HeroTurnTakerController httc = FindHeroTurnTakerController(hero.ToHero());
            CardController cc = null;
            Dictionary<Card, bool> initialAllowCoroutineDict = new Dictionary<Card, bool>();
            foreach (Card character in httc.CharacterCards)
            {
                cc = FindCardController(character);

                initialAllowCoroutineDict.Add(character, cc.AllowFastCoroutinesDuringPretend);
                cc.AllowFastCoroutinesDuringPretend = false;
            }

            List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
            IEnumerator coroutine = RemoveTokensFromPoolNewDecisionMaker(elementPool, 1, storedResults: storedResults, optional: true, httc: httc, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidRemoveTokens(storedResults))
            {
                //Select a damage type.
                List<SelectDamageTypeDecision> storedDamageTypeResults = new List<SelectDamageTypeDecision>();
                coroutine = base.GameController.SelectDamageType(httc, storedDamageTypeResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                DamageType damageType = GetSelectedDamageType(storedDamageTypeResults).Value;


                coroutine = GameController.ChangeDamageType(dd, damageType, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }

            foreach (Card character in httc.CharacterCards)
            {
                cc = FindCardController(character);
                cc.AllowFastCoroutinesDuringPretend = initialAllowCoroutineDict[character];
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One player may play a card.

                        IEnumerator coroutine3 = SelectHeroToPlayCard(DecisionMaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }

                        break;
                    }
                case 1:
                    {
                        // Add 1 token to your element pool.
                        TokenPool elementPool = GetElementTokenPool();
                        IEnumerator coroutine2 = base.GameController.AddTokensToPool(elementPool, 1, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        // Spend 1 token from your element pool. If you do, 1 hero deals 1 target 3 damage of any type.
                        TokenPool elementPool = GetElementTokenPool();
                        List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
                        IEnumerator coroutine = base.GameController.RemoveTokensFromPool(elementPool, 1, storedResults: storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidRemoveTokens(storedResults))
                        {
                            //select a hero character card
                            List<SelectTurnTakerDecision> storedTurnTakerResults = new List<SelectTurnTakerDecision>();
                            IEnumerator coroutine3 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.CardToDealDamage, false, false, storedTurnTakerResults, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                            SelectTurnTakerDecision selectTurnTakerDecision = storedTurnTakerResults.FirstOrDefault();
                            if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null)
                            {
                                Card source = selectTurnTakerDecision.SelectedTurnTaker.CharacterCard;
                                HeroTurnTakerController httc = FindHeroTurnTakerController(selectTurnTakerDecision.SelectedTurnTaker.ToHero());

                                //Select a damage type.
                                List<SelectDamageTypeDecision> storedDamageTypeResults = new List<SelectDamageTypeDecision>();
                                coroutine = base.GameController.SelectDamageType(httc, storedDamageTypeResults, cardSource: GetCardSource());
                                if (base.UseUnityCoroutines)
                                {
                                    yield return base.GameController.StartCoroutine(coroutine);
                                }
                                else
                                {
                                    base.GameController.ExhaustCoroutine(coroutine);
                                }
                                DamageType damageType = GetSelectedDamageType(storedDamageTypeResults).Value;

                                //selected hero deals damage of the selected type
                                coroutine3 = base.GameController.SelectTargetsAndDealDamage(httc, new DamageSource(base.GameController, source), 3, damageType, new int?(1), false, new int?(1), cardSource: GetCardSource());
                                if (base.UseUnityCoroutines)
                                {
                                    yield return base.GameController.StartCoroutine(coroutine3);
                                }
                                else
                                {
                                    base.GameController.ExhaustCoroutine(coroutine3);
                                }
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

        public IEnumerator RemoveTokensFromPoolNewDecisionMaker(TokenPool pool, int numberOfTokens, List<RemoveTokensFromPoolAction> storedResults = null, bool optional = false, GameAction gameAction = null, HeroTurnTakerController httc = null, IEnumerable<Card> associatedCards = null, CardSource cardSource = null)
        {
            bool proceed = true;

            if (httc == null)
            {
                httc = DecisionMaker;
            }

            if (optional)
            {
                proceed = false;
                SelectionType type = SelectionType.RemoveTokens;
                if (pool.CurrentValue < numberOfTokens)
                {
                    type = SelectionType.RemoveTokensToNoEffect;
                }
                YesNoAmountDecision yesNo = new YesNoAmountDecision(GameController, httc, type, numberOfTokens, upTo: false, requireUnanimous: false, gameAction, associatedCards: associatedCards, cardSource);
                IEnumerator coroutine = GameController.MakeDecisionAction(yesNo);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (yesNo.Answer.HasValue)
                {
                    proceed = yesNo.Answer.Value;
                }
            }
            if (proceed)
            {
                RemoveTokensFromPoolAction removeTokensFromPoolAction = ((cardSource == null) ? new RemoveTokensFromPoolAction(GameController, pool, numberOfTokens) : new RemoveTokensFromPoolAction(cardSource, pool, numberOfTokens));
                storedResults?.Add(removeTokensFromPoolAction);
                IEnumerator coroutine2 = DoAction(removeTokensFromPoolAction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }
    }
}