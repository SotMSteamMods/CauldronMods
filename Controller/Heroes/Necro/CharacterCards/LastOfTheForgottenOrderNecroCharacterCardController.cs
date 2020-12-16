using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;

namespace Cauldron.Necro
{
    public class LastOfTheForgottenOrderNecroCharacterCardController : HeroCharacterCardController
    {
        public LastOfTheForgottenOrderNecroCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //PowerNumerals Required on powers
            int target = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 1);
            int[] powerNumerals = new int[]
            {
                target, amount
            };

            //The next time an undead target is destroyed, 1 hero deals a target 1 fire damage and draws a card.
            WhenCardIsDestroyedStatusEffect effect = new WhenCardIsDestroyedStatusEffect(base.Card, "DealDamageAndDrawResponse", "The next time an undead target is destroyed, 1 hero deals a target 1 fire damage and draws a card", new TriggerType[] { TriggerType.DealDamage, TriggerType.DrawCard }, DecisionMaker.HeroTurnTaker, base.Card, powerNumerals);
            effect.NumberOfUses = 1;
            effect.CanEffectStack = true;
            effect.CardDestroyedCriteria.IsTarget = true;
            effect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string>() { "undead" };
            IEnumerator coroutine3 = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        //Destroy an environment card.
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController,
                            new LinqCardCriteria((Card c) => c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment"),
                            optional: false,
                            cardSource: GetCardSource());
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
                        //One player may play 2 random cards from their hand now.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine3 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.PutIntoPlay, false, false, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        if (DidSelectTurnTaker(storedResults))
                        {
                            TurnTaker tt = GetSelectedTurnTaker(storedResults);
                            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
                            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
                            coroutine3 = GameController.MakeYesNoCardDecision(httc, SelectionType.PlayCard, base.Card, storedResults: storedYesNoResults, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                            if (DidPlayerAnswerYes(storedYesNoResults))
                            {

                                coroutine3 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, tt.ToHero().Hand, true, false, false, new LinqCardCriteria((Card c) => true), 2, shuffleBeforehand: true);
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

        public IEnumerator DealDamageAndDrawResponse(DestroyCardAction dca, HeroTurnTaker htt, WhenCardIsDestroyedStatusEffect effect, int[] powerNumerals = null)
        {
            int target = powerNumerals?[0] ?? 1;
            int amount = powerNumerals?[1] ?? 1;

            if (dca.WasCardDestroyed)
            {
                IEnumerator coroutine = GameController.ExpireStatusEffect(effect, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //1 hero deals a target 1 fire damage and draws a card.
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage,
                    new LinqCardCriteria((Card c) => c.IsInPlay && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero character", useCardsSuffix: false),
                    storedResults, false,
                    cardSource: GetCardSource());
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
                    Card selectedCard = GetSelectedCard(storedResults);
                    coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, selectedCard), amount, DamageType.Fire, target, optional: false, target, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);

                    }

                    coroutine = DrawCard(selectedCard.Owner.ToHero());
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
    }
}
