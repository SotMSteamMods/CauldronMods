using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
    public class FutureLadyOfTheWoodCharacterCardController : LadyOfTheWoodUtilityCharacterCardController
    {
        public FutureLadyOfTheWoodCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //Destroy a season.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsSeason(c), "season"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Play a season...
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, storedResults: storedResults, cardCriteria: new LinqCardCriteria((Card c) => IsSeason(c), "season"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // ...and change the next damage dealt by {LadyOfTheWood} to a type listed on it.
            if (DidPlayCards(storedResults))
            {
                Card season = storedResults.First().CardToPlay;
                DamageType? dt = GetDamageTypeFromSeason(season);
                if (dt != null)
                {
                    DamageType damageType = dt.Value;
                    ChangeDamageTypeStatusEffect effect = new ChangeDamageTypeStatusEffect(damageType);
                    effect.NumberOfUses = 1;
                    effect.SourceCriteria.IsSpecificCard = base.Card;
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
            }

            // You may use a power
            coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: GetCardSource());
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
                        //One player may draw a card.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        //One target deals itself 1 cold damage.
                        IEnumerable<Card> choices = FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.IsTarget);
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTarget, choices, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        Card card = storedResults.Select((SelectCardDecision d) => d.SelectedCard).FirstOrDefault();
                        if (card != null)
                        {
                            IEnumerator coroutine3 = DealDamage(card, card, 1, DamageType.Cold, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //Select 1 of your season cards, its text affects all heroes until your next turn.

                        //select a season card from under this card
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine4 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ActivateAbility, new LinqCardCriteria((Card c) => base.Card.UnderLocation.HasCard(c)), storedResults, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine4);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            //Trigger the appropriate Status Effects based on the selected season
                            Card selectedSeason = GetSelectedCard(storedResults);
                            IEnumerator coroutine5 = null;
                            switch (selectedSeason.Identifier)
                            {
                                case "Fall":
                                    {
                                        coroutine5 = AddFallResponse();
                                        break;
                                    }
                                case "Winter":
                                    {
                                        coroutine5 = AddWinterResponse();
                                        break;
                                    }
                                case "Spring":
                                    {
                                        coroutine5 = AddSpringResponse();
                                        break;
                                    }
                                case "Summer":
                                    {
                                        coroutine5 = AddSummerResponse();
                                        break;
                                    }
                            }

                            if (coroutine5 != null)
                            {
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
                        break;
                    }
            }

            yield break;
        }

        private bool IsSeason(Card card)
        {
            return card != null && card.DoKeywordsContain("season");
        }

        private DamageType? GetDamageTypeFromSeason(Card season)
        {

            switch (season.Identifier)
            {
                case "Fall":
                    {
                        return DamageType.Lightning;
                    }
                case "Winter":
                    {
                        return DamageType.Cold;
                    }
                case "Spring":
                    {
                        return DamageType.Toxic;
                    }
                case "Summer":
                    {
                        return DamageType.Fire;
                    }
            }

            return null;
        }

        public override bool KeepUnderCardOnIncapacitation(Card card)
        {
            bool flag = card.DoKeywordsContain("season", evenIfUnderCard: true) && card.Location.OwnerCard == base.Card;
            if (!flag)
            {
                flag = base.KeepUnderCardOnIncapacitation(card);
            }
            return flag;
        }

        protected override IEnumerator RemoveCardsFromGame(IEnumerable<Card> cards)
        {
            cards = cards.Where((Card c) => !c.IsCharacter && !c.IsMissionCard && !c.Location.IsOffToTheSide);
            IEnumerator coroutine = base.GameController.BulkMoveCards(base.TurnTakerController, cards, base.TurnTakerControllerWithoutReplacements.TurnTaker.OutOfGame);
            Log.Debug("All of " + base.TurnTakerControllerWithoutReplacements.Name + "'s cards are removed from the game except Season cards.");
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator coroutine2 = base.GameController.SendMessageAction("Put all season cards under " + base.Card.Title + ".", Priority.High, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            IEnumerable<Card> seasons = FindCardsWhere((Card c) => IsSeason(c));
            seasons = DistinctBy(seasons, c => c.Identifier);

            coroutine = base.GameController.BulkMoveCards(base.TurnTakerController, seasons, base.Card.UnderLocation, toBottom: false, performBeforeDestroyActions: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            foreach (Card item in seasons)
            {
                CardController inhibitedCard = FindCardController(item);
                base.GameController.AddInhibitorException(inhibitedCard, (GameAction ga) => true);
                List<string> list = new List<string>();
                list.Add(base.TurnTaker.Identifier);
                list.Add(item.Identifier);
                base.GameController.AddCardPropertyJournalEntry(item, "OverrideTurnTaker", list);
            }
        }

        private IEnumerator AddSpringResponse()
        {
            //Whenever a hero deals toxic damage to a target, they regain that much HP.
            OnDealDamageStatusEffect effect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(SpringGainHPReponse), "Whenever a hero deals toxic damage to a target, they regain that much HP.", new TriggerType[]
                {
                TriggerType.GainHP
                }, base.TurnTaker, base.Card);
            effect.SourceCriteria.IsHeroCharacterCard = true;
            effect.DamageTypeCriteria.AddType(DamageType.Toxic);
            effect.UntilStartOfNextTurn(base.TurnTaker);
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator SpringGainHPReponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //they regain that much HP
            int amountToGain = dd.Amount;
            if (dd.DamageSource != null && dd.DidDealDamage)
            {
                Card source = dd.DamageSource.Card;
                IEnumerator coroutine = base.GameController.GainHP(source, amountToGain, cardSource: GetCardSource());
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

        private IEnumerator AddSummerResponse()
        {
            //Increase fire damage dealt by heroes by 2
            IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(2);
            effect.SourceCriteria.IsHeroCharacterCard = true;
            effect.DamageTypeCriteria.AddType(DamageType.Fire);
            effect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AddWinterResponse()
        {
            //Whenever a hero deals cold damage to a target, they draw a card.
            OnDealDamageStatusEffect effect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(WinterDrawCardReponse), "Whenever a hero deals cold damage to a target, they draw a card.", new TriggerType[]
                {
                TriggerType.DrawCard
                }, base.TurnTaker, base.Card);
            effect.SourceCriteria.IsHeroCharacterCard = true;
            effect.UntilStartOfNextTurn(base.TurnTaker);
            effect.DamageTypeCriteria.AddType(DamageType.Cold);
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator WinterDrawCardReponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //they draw a card
            if (dd.DamageSource != null && dd.DidDealDamage)
            {
                HeroTurnTaker source = dd.DamageSource.Card.Owner.ToHero();
                IEnumerator coroutine = base.DrawCard(source);
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

        private IEnumerator AddFallResponse()
        {
            //Whenever a hero deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.
            OnDealDamageStatusEffect effect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(FallReduceDamageReponse), "Whenever a hero deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.", new TriggerType[]
                {
                TriggerType.ReduceDamage
                }, base.TurnTaker, base.Card);
            effect.SourceCriteria.IsHeroCharacterCard = true;
            effect.DamageTypeCriteria.AddType(DamageType.Lightning);
            effect.UntilStartOfNextTurn(base.TurnTaker);
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator FallReduceDamageReponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //Reduce damage dealt by that target by 1 until the start of their next turn.
            if (dd.DamageSource != null && dd.DidDealDamage)
            {
                ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
                reduceDamageStatusEffect.UntilStartOfNextTurn(dd.DamageSource.Card.Owner);
                IEnumerator coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
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

        public IEnumerable<Card> DistinctBy(IEnumerable<Card> source, Func<Card, string> keySelector)
        {
            HashSet<string> seenKeys = new HashSet<string>();
            foreach (Card element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

    }
}
