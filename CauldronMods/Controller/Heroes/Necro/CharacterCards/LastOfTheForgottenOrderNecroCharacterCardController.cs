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

        private enum CustomMode
        {
            SelectTurnTakerDecision,
            YesNoDecision
        }

        private CustomMode customMode { get; set; }

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
            effect.CanEffectStack = true;
            effect.CardDestroyedCriteria.IsTarget = true;
            effect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string>() { "undead" };
            effect.CanEffectStack = true;
            effect.DoesDealDamage = true;
            //numberOfUses = 1 means the status effect expires before power modifiers get to see the damage
            //instead we'll expire it in the response

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
                        customMode = CustomMode.SelectTurnTakerDecision;
                        IEnumerator coroutine3 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, false, false, storedResults, heroCriteria: new LinqTurnTakerCriteria(turnTaker => turnTaker.IsHero && turnTaker.ToHero().NumberOfCardsInHand > 0), cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        if (!DidSelectTurnTaker(storedResults))
                        {
                            yield break;
                        }

                        TurnTaker tt = GetSelectedTurnTaker(storedResults);
                        HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
                        List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
                        customMode = CustomMode.YesNoDecision;
                        coroutine3 = GameController.MakeYesNoCardDecision(httc, SelectionType.Custom, base.Card, storedResults: storedYesNoResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        if (!DidPlayerAnswerYes(storedYesNoResults))
                        {
                            yield break;
                        }

                        coroutine3 = Play2RandomCardsFromHand(tt);
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
            }
            yield break;
        }

        private IEnumerator Play2RandomCardsFromHand(TurnTaker tt)
        {
            HeroTurnTaker htt = tt.ToHero();
            TurnTakerController ttc = FindTurnTakerController(tt);
            CardSource cardSource = GetCardSource();
            IEnumerator coroutine;
            int cardToPlayIndex;
            Card cardToPlay;
            IEnumerable<Card> playableCards;
            for(int i=0; i < 2; i++)
            {
                playableCards = htt.Hand.Cards.Where(c => FindCardController(c).CanBePlayedNow);
                if (playableCards.Count() == 0)
                {
                    string moreWord = i > 0 ? "more " : "";
                    coroutine = GameController.SendMessageAction($"{tt.Name} has no {moreWord} cards that can be played!", Priority.Medium, cardSource, showCardSource: true);
                    yield break;
                }

                cardToPlayIndex = Game.RNG.Next(0, playableCards.Count());
                cardToPlay = playableCards.ElementAt(cardToPlayIndex);

                coroutine = GameController.SendMessageAction($"{TurnTaker.NameRespectingVariant} forces {tt.Name} to play {cardToPlay.Title}!", Priority.Medium, cardSource, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.PlayCard(ttc, cardToPlay, cardSource: cardSource);
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

        public IEnumerator DealDamageAndDrawResponse(DestroyCardAction dca, HeroTurnTaker htt, WhenCardIsDestroyedStatusEffect effect, int[] powerNumerals = null)
        {
            int target = powerNumerals?[0] ?? 1;
            int amount = powerNumerals?[1] ?? 1;

            if (dca.WasCardDestroyed && effect.NumberOfUses == null)
            {
                effect.NumberOfUses = 1;

                //1 hero deals a target 1 fire damage and draws a card.
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage,
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
                    coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, selectedCard), amount, DamageType.Fire, target, optional: false, target, cardSource: GetCardSource(effect));
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

                coroutine = GameController.ExpireStatusEffect(effect, GetCardSource());
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if(customMode == CustomMode.SelectTurnTakerDecision)
            {
                return new CustomDecisionText("Select a player to play 2 random cards from their hand.", "They are selecting a player to play 2 random cards from their hand.", "Vote for a player to play 2 random cards from their hand.", "play 2 random cards from hand");

            }

            if (customMode == CustomMode.YesNoDecision)
            {
                return new CustomDecisionText("Do you want to play 2 random cards from your hand?", "Should they play 2 random cards from their hand?", "Vote for if they should play 2 random cards from their hand?", "play 2 random cards from their hand");
            }

            return base.GetCustomDecisionText(decision);


        }
    }
}
