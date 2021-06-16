using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.LadyOfTheWood
{
    public class MinistryOfStrategicScienceLadyOfTheWoodCharacterCardController : HeroCharacterCardController
    {
        public static readonly string LadyOfTheWoodElementPoolIdentifier = "LadyOfTheWoodElementPool";
        public readonly string LadyOfTheWoodIdentifier = "LadyOfTheWood";

        public MinistryOfStrategicScienceLadyOfTheWoodCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //CardWithoutReplacements.TokenPools.ReorderTokenPool(LadyOfTheWoodElementPoolIdentifier);

            AllowFastCoroutinesDuringPretend = false;
            SpecialString specialString = base.SpecialStringMaker.ShowTokenPool(GetElementTokenPool());
            specialString.ShowWhileIncapacitated = true;
        }

        public override void AddTriggers()
        {
            TokenPool elementPool = GetElementTokenPool();
            //When {LadyOfTheWood} would deal damage, you may change its type by spending a token.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == Card && elementPool.CurrentValue > 0, SpendTokenResponse, new TriggerType[]
            {
                TriggerType.ModifyTokens,
                TriggerType.ChangeDamageType
            }, TriggerTiming.Before, isActionOptional: true);
        }

        private TokenPool GetElementTokenPool()
        {
            TokenPool elementPool = Card.FindTokenPool(LadyOfTheWoodElementPoolIdentifier);
            if (elementPool is null)
            {
                elementPool = CardWithoutReplacements.FindTokenPool(LadyOfTheWoodElementPoolIdentifier);
            }

            return elementPool;
        }

        private IEnumerator SpendTokenResponse(DealDamageAction dd)
        {
            TokenPool elementPool = GetElementTokenPool();

            List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
            IEnumerator coroutine = base.GameController.RemoveTokensFromPool(elementPool, 1, storedResults: storedResults, optional: true, cardSource: base.GetCardSource());
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
                coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedDamageTypeResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                var damageType = GetSelectedDamageType(storedDamageTypeResults);
                if (damageType.HasValue)
                {
                    coroutine = GameController.ChangeDamageType(dd, damageType.Value, GetCardSource());
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
            yield break;
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

        //override flip response to remove resetting token pools
        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            if (base.CardWithoutReplacements.IsFlipped)
            {
                yield break;
            }
            IEnumerable<Card> enumerable = from c in base.TurnTakerControllerWithoutReplacements.TurnTaker.GetAllCards()
                                           where c.Location.IsEnvironment && (c.Location.IsDeck || c.Location.IsTrash)
                                           select c;
            if (enumerable.Count() > 0)
            {
                IEnumerator coroutine = RemoveCardsFromGame(enumerable);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.CardWithoutReplacements.IsTarget)
            {
                RemoveTargetAction action = new RemoveTargetAction(base.GameController, base.CardWithoutReplacements);
                IEnumerator coroutine2 = DoAction(action);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            if (base.TurnTakerControllerWithoutReplacements.IsIncapacitatedOrOutOfGame)
            {
                base.GameController.RemoveTriggers((ITrigger t) => t.CardSource != null && t.CardSource.CardController.TurnTakerControllerWithoutReplacements == base.TurnTakerControllerWithoutReplacements && !t.IsStatusEffect && !t.IsOutOfPlayTrigger && t.CardSource != null && (t.CardSource.CardController == this || !t.CardSource.Card.IsInPlayAndNotUnderCard));
                IEnumerable<Card> cards = base.TurnTakerControllerWithoutReplacements.TurnTaker.GetAllCards();
                List<Card> list = new List<Card>();
                list.AddRange(cards.Where((Card c) => !c.IsHeroCharacterCard).SelectMany((Card c) => c.UnderLocation.Cards.Where((Card co) => co.Owner != base.TurnTakerControllerWithoutReplacements.TurnTaker)));
                if (base.TurnTakerControllerWithoutReplacements.TurnTaker.IsHero)
                {
                    HeroTurnTaker heroTurnTaker = base.TurnTakerControllerWithoutReplacements.ToHero().HeroTurnTaker;
                    list.AddRange(from c in heroTurnTaker.Hand.Cards.Union(heroTurnTaker.Deck.Cards).Union(heroTurnTaker.Trash.Cards)
                                  where c.Owner != base.TurnTakerControllerWithoutReplacements.TurnTaker
                                  select c);
                }
                foreach (Card item in list)
                {
                    MoveCardDestination trashDestination = FindCardController(item).GetTrashDestination();
                    IEnumerator coroutine3 = base.GameController.MoveCard(base.TurnTakerController, item, trashDestination.Location, trashDestination.ToBottom);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine3);
                    }
                }
                if (base.Game.IsOblivAeonMode)
                {
                    IEnumerable<Card> enumerable2 = FindCardsWhere((Card c) => c.IsObjective && c.IsInLocation(base.TurnTakerControllerWithoutReplacements.TurnTaker.PlayArea));
                    Log.Debug("Objectives: " + enumerable2.ToRecursiveString());
                    if (enumerable2.Count() > 0)
                    {
                        TurnTaker turnTaker = FindTurnTakersWhere((TurnTaker tt) => tt.Identifier == "OblivAeon", ignoreBattleZone: true).FirstOrDefault();
                        IEnumerator coroutine4 = base.GameController.BulkMoveCards(base.TurnTakerController, enumerable2, turnTaker.FindSubDeck("MissionDeck"), toBottom: true, performBeforeDestroyActions: true, null, isDiscard: false, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine4);
                        }
                    }
                    IEnumerable<Card> enumerable3 = FindCardsWhere((Card c) => c.DoKeywordsContain("reward", evenIfUnderCard: true, evenIfFaceDown: true) && !c.IsOutOfGame && c.Owner == base.TurnTakerControllerWithoutReplacements.TurnTaker && (!c.IsUnderCard || !c.Location.OwnerCard.IsIncapacitated), realCardsOnly: false, null, ignoreBattleZone: true);
                    if (enumerable3.Count() > 0)
                    {
                        IEnumerator coroutine5 = base.GameController.BulkMoveCards(base.TurnTakerController, enumerable3, base.Card.UnderLocation);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine5);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine5);
                        }
                    }
                }
                IEnumerator coroutine6 = RemoveCardsFromGame(cards);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine6);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine6);
                }

                IEnumerator coroutine7 = base.GameController.UpdateTurnPhasesForTurnTaker(base.TurnTakerControllerWithoutReplacements, incapacitated: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine7);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine7);
                }
            }
            else
            {
                RemoveAllTriggers();
            }
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