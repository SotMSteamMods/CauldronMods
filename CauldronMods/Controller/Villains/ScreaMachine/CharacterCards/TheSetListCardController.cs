using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class TheSetListCardController : ScreaMachineUtilityCharacterCardController
    {
        public TheSetListCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsUnderCard(this.Card);
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }


        public IEnumerator RevealTopCardOfTheSetList()
        {
            List<Card> dummy = new List<Card>();
            var coroutine = GameController.RevealCards(TurnTakerController, this.Card.UnderLocation, 1, dummy, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator TheSetListRevealProcess(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                var keywords = new HashSet<string>(GameController.GetAllKeywords(card), StringComparer.OrdinalIgnoreCase);
                var sharesAKeyword = FindCardsWhere(new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && GameController.GetAllKeywords(c).Any(k => keywords.Contains(k))), GetCardSource()).Any();
                var firstCC = FindCardController(card) as ScreaMachineBandCardController;
                Card cardToPlay = card;
                IEnumerator coroutine;
                if (!sharesAKeyword)
                {
                    coroutine = GameController.MoveCard(TurnTakerController, card, Card.UnderLocation, toBottom: true, playCardIfMovingToPlayArea: false, flipFaceDown: true, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    cardToPlay = this.Card.UnderLocation.TopCard;
                }

                var secondCC = FindCardController(cardToPlay) as ScreaMachineBandCardController;
                coroutine = TheSetListFlavorMessage(firstCC, secondCC);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //precheck play, so we can flip first
                var cc = FindCardController(cardToPlay);
                if (cardToPlay.IsFlipped && GameController.CanPlayCard(cc) == CanPlayCardResult.CanPlay)
                {
                    coroutine = GameController.FlipCard(cc, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, reassignPlayIndex: true, evenIfAlreadyInPlay: true, cardSource: GetCardSource());
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

        private IEnumerator TheSetListFlavorMessage(ScreaMachineBandCardController firstCard, ScreaMachineBandCardController secondCard)
        {
            if (firstCard != null && secondCard != null)
            {
                Card card;
                string message;
                bool sendMessage = false;
                if (firstCard.Member == secondCard.Member)
                {
                    if (firstCard.IsBandmateInPlay)
                        sendMessage = true;

                    var bandMate = firstCard.GetBandmate();
                    if (firstCard.Card != secondCard.Card)
                    {
                        //revealed card bandmate doesn't have cards already in play, but the played card will start things off
                        card = secondCard.Card;
                        message = $"[b]{bandMate.Title}[/b] is starting to feel it!";
                    }
                    else
                    {
                        //revealed card bandmate has cards already in play   
                        int count = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain(firstCard.Member.GetKeyword())).Count();
                        card = firstCard.Card;
                        switch (count)
                        {
                            case 1: //1 already, revealed card is the second
                                message = $"[b]{bandMate.Title}[/b] is ramping it up!";
                                break;
                            case 2: //2 already, revealed card is the third, will flip
                                message = $"The music [b]surges[/b], this is [b]{bandMate.Title}[/b] moment!";
                                break;
                            default:
                                message = $"[b]{bandMate.Title}[/b] is ramping it up!";
                                break;
                        }
                    }
                }
                else
                {
                    //first card doesn't have any cards already in play, playing second card.
                    if (secondCard.IsBandmateInPlay)
                        sendMessage = true;

                    card = secondCard.Card;
                    var bandMate = secondCard.GetBandmate();
                    int count = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain(secondCard.Member.GetKeyword())).Count();

                    if (firstCard.IsBandmateInPlay)
                    {
                        message = $"{firstCard.GetBandmate().Title} is keeping it mellow, while ";
                    }
                    else
                    {
                        message = "";
                    }
                    switch (count)
                    {
                        case 0: //0 already, play card is the first
                            message += $"[b]{secondCard.GetBandmate().Title}[/b] steps into the lime-light!";
                            break;
                        case 1: //1 already, revealed card is the second
                            message += $"[b]{bandMate.Title}[/b] is ramping it up!";
                            break;
                        case 2: //2 already, revealed card is the third, will flip
                            message = $"The music [b]surges[/b], this is [b]{bandMate.Title}[/b] moment!";
                            break;
                        default:
                            message = $"[b]{bandMate.Title}[/b] is ramping it up!";
                            break;
                    }
                }

                if (sendMessage)
                {
                    var coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { card });
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

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => FlipThisCharacterCardResponse(pca), TriggerType.FlipCard));
            }
            else
            {
                AddSideTrigger(AddTrigger<RevealCardsAction>(rca => rca.SearchLocation == this.Card.UnderLocation, rca => TheSetListRevealProcess(rca.RevealedCards), TriggerType.PlayCard, TriggerTiming.After));
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => RevealTopCardOfTheSetList(), TriggerType.RevealCard));
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker && FindCardsWhere(c => IsVillainTarget(c) && c.IsCharacter && c.IsInPlay).Count() <= (Game.H - 2), pca => EndOfTurnDamageAndPlay(), TriggerType.DealDamage));
                AddSideTrigger(AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && !IsVillain(md.CardSource.Card), this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before));
                if (Game.IsAdvanced)
                {
                    AddSideTrigger(AddTrigger<FlipCardAction>(fca => IsVillainTarget(fca.CardToFlip.Card), fca => FlipCardResponse(), TriggerType.DestroyCard, TriggerTiming.After));
                }
            }
        }

        private IEnumerator FlipCardResponse()
        {
            var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && c.IsHero && (c.IsOngoing || IsEquipment(c)), "hero ongoing or equipment"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator EndOfTurnDamageAndPlay()
        {
            List<Card> highest = new List<Card>();
            var fake = new DealDamageAction(GetCardSource(), new DamageSource(GameController, TurnTaker), null, 2, DamageType.Sonic);
            var coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, c => IsVillainTarget(c) && c.IsCharacter && c.IsInPlay, highest,
                            dealDamageInfo: new[] { fake },
                            evenIfCannotDealDamage: true,
                            cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DealDamage(DecisionMaker, highest.First(), c => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, 2, DamageType.Sonic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove card under this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => Card.UnderLocation.HasCard(d.SelectedCard));
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            //cards under this card cannot be affected by non villain cards
            if (Card.UnderLocation.HasCard(card) && !IsVillain(cardSource.Card))
            {
                return false;
            }
            return true;
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            //actions from non-villain cards cannot affect cards under this card

            bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => !IsVillain(c), (Card c) => Card.UnderLocation.HasCard(c));

            if (flag != null && flag.Value)
            {
                return false;
            }
            
            return true;
        }
    }
}