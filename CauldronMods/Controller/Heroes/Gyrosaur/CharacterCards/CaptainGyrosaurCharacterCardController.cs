using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class CaptainGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public CaptainGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(Card.UnderLocation, new LinqCardCriteria());
        }
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            //"Put the top card of your deck beneath {Gyrosaur}, face up. 
            if(!TurnTaker.Deck.HasCards)
            {
                coroutine = GameController.SendMessageAction($"There are no cards in ${DecisionMaker.TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource());
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

            var moveStorage = new List<MoveCardAction>();
            coroutine = GameController.MoveCard(DecisionMaker, DecisionMaker.TurnTaker.Deck.TopCard, CharacterCard.UnderLocation, storedResults: moveStorage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var movedCard = moveStorage.FirstOrDefault()?.CardToMove;
            if(movedCard == null || movedCard.Location != CharacterCard.UnderLocation)
            {
                yield break;
            }

            //When she deals damage, play or draw it."
            var playOrDrawEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(PlayOrDrawUnderCard), $"When {DecisionMaker.Name} deals damage, they play or draw {movedCard.Title}.", new TriggerType[] { TriggerType.PlayCard, TriggerType.DrawCard }, DecisionMaker.TurnTaker, Card);
            playOrDrawEffect.SourceCriteria.IsSpecificCard = CharacterCard;
            playOrDrawEffect.UntilTargetLeavesPlay(CharacterCard);
            playOrDrawEffect.CardMovedExpiryCriteria.Card = movedCard;
            playOrDrawEffect.BeforeOrAfter = BeforeOrAfter.After;
            playOrDrawEffect.DamageAmountCriteria.GreaterThan = 0;

            coroutine = AddStatusEffect(playOrDrawEffect);
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

        public IEnumerator PlayOrDrawUnderCard(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //prevent the effect from going off in the middle of it already being activated
            if (effect.CardFlippedExpiryCriteria.Card == null)
            {
                var affectedCard = effect.CardMovedExpiryCriteria.Card;
                effect.CardFlippedExpiryCriteria.Card = affectedCard;

                var heroTTC = FindHeroTurnTakerController(hero.ToHero());
                var originalLocation = affectedCard.Location;
                var effectSource = FindCardController(effect.CardSource).GetCardSource(effect);
                var functions = new List<Function>
                {
                    new Function(heroTTC, $"Play {affectedCard.Title}", SelectionType.PlayCard, () => GameController.PlayCard(heroTTC, affectedCard, reassignPlayIndex: true, cardSource: effectSource), onlyDisplayIfTrue: GameController.CanPlayCard(FindCardController(affectedCard)) == CanPlayCardResult.CanPlay, forcedActionMessage: $"{affectedCard.Title} cannot be drawn, so it will be played."),
                    new Function(heroTTC, $"Draw {affectedCard.Title}", SelectionType.DrawCard, () => DrawCardFromUnder(heroTTC, affectedCard, effectSource), onlyDisplayIfTrue: GameController.CanDrawCards(heroTTC, effectSource), forcedActionMessage: $"{affectedCard.Title} cannot be played, so it will be drawn")
                };

                var selectFunction = new SelectFunctionDecision(GameController, heroTTC, functions, false, noSelectableFunctionMessage: $"{affectedCard.Title} can neither be played nor drawn.", associatedCards: new Card[] { affectedCard }, cardSource: effectSource);
                IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction, associatedCards: new Card[] { affectedCard });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                effect.CardFlippedExpiryCriteria = null;
                if (affectedCard.Location != originalLocation)
                {
                    coroutine = GameController.ExpireStatusEffect(effect, effectSource);
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

        private IEnumerator DrawCardFromUnder(HeroTurnTakerController heroTTC, Card cardToDraw, CardSource drawSource)
        {
            //this is 'illegal', since it does a thing without telling the GameController/engine
            //but DrawCard is hardcoded to only draw from the top of the deck
            //so we do this to fake drawing a card from under another
            var originalLocation = cardToDraw.Location;
            heroTTC.TurnTaker.MoveCard(cardToDraw, heroTTC.TurnTaker.Deck);

            var drawStorage = new List<DrawCardAction>();
            IEnumerator coroutine = GameController.DrawCard(heroTTC.HeroTurnTaker, storedResults: drawStorage, cardSource: drawSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //now we deal with sneaking the card back into its proper place, if needed
            if(!DidDrawCards(drawStorage) && cardToDraw == heroTTC.TurnTaker.Deck.TopCard)
            {
                heroTTC.TurnTaker.MoveCard(cardToDraw, originalLocation);
            }
            yield break;
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
                        //"One hero may use a power now.",
                        coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                        //"Select a target. Increase the next damage dealt to it by 2."
                        var validTargets = AllCards.Where((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
                        var selectCard = new SelectCardDecision(GameController, DecisionMaker, SelectionType.SelectTarget, validTargets, cardSource: GetCardSource());
                        coroutine = GameController.SelectCardAndDoAction(selectCard, IncreaseNextDamageToChosen);
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

        private IEnumerator IncreaseNextDamageToChosen(SelectCardDecision scd)
        {
            Card target = scd.SelectedCard;
            if(target == null)
            {
                yield break;
            }

            var increaseEffect = new IncreaseDamageStatusEffect(2);
            increaseEffect.NumberOfUses = 1;
            increaseEffect.TargetCriteria.IsSpecificCard = target;
            increaseEffect.UntilTargetLeavesPlay(target);
            increaseEffect.CardSource = Card;

            IEnumerator coroutine = AddStatusEffect(increaseEffect);
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
