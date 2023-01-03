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
            SpecialStringMaker.ShowVillainTargetWithHighestHP().Condition = () => Card.IsFlipped && FindCardsWhere(c => IsVillainTarget(c) && c.IsCharacter && c.IsInPlay).Count() <= (Game.H - 2);
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
        private int ExistingCardsNeededToFlip => Game.IsChallenge ? 1 : 2;
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
                        message = $"[b]{bandMate.Title}[/b] is starting to feel it and plays a {secondCard.Member.GetKeyword()} card!";
                    }
                    else
                    {
                        //revealed card bandmate has cards already in play   
                        int count = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain(firstCard.Member.GetKeyword())).Count();
                        card = firstCard.Card;
                        if(count == ExistingCardsNeededToFlip)
                        {
                            //we have reached the number needed to flip
                            message = $"The music [b]surges[/b] and a {firstCard.Member.GetKeyword()} card is played! This is [b]{bandMate.Title}[/b] moment!";
                        }
                        else
                        { 
                            switch (count)
                            {
                                case 1: //1 already, revealed card is the second, is not challenge mode
                                    message = $"[b]{bandMate.Title}[/b] is ramping it up and plays a {firstCard.Member.GetKeyword()} card!";
                                    break;
                                case 2: //must be in challenge mode and already flipped to get here
                                    message = $"The music [b]rages[/b] and a {firstCard.Member.GetKeyword()} card is played! [b]{bandMate.Title}[/b] is going into overdrive!";
                                    break;
                                default:
                                    message = $"[b]{bandMate.Title}[/b] is ramping it up and plays a {firstCard.Member.GetKeyword()} card!";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    //first card doesn't have any cards already in play, playing second card.
                    if (secondCard.IsBandmateInPlay)
                    {
                        sendMessage = true;
                    }

                    card = secondCard.Card;
                    var bandMate = secondCard.GetBandmate();
                    int count = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain(secondCard.Member.GetKeyword())).Count();

                    if (firstCard.IsBandmateInPlay)
                    {
                        var coroutineFlip = GameController.FlipCard(firstCard, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutineFlip);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutineFlip);
                        }

                        message = $"{firstCard.GetBandmate().Title} is keeping it mellow since there aren't any {firstCard.Member.GetKeyword()} cards in play!";
                        var coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { firstCard.Card });
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        coroutineFlip = GameController.FlipCard(firstCard, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutineFlip);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutineFlip);
                        }

                    }
                    
                    message = "";
                    if (count == ExistingCardsNeededToFlip)
                    {
                        //we have reached the number needed to flip
                        message = $"The music [b]surges[/b] and a {secondCard.Member.GetKeyword()} card is played! This is [b]{bandMate.Title}[/b] moment!";
                    }
                    else
                    {
                        switch (count)
                        {
                            case 0: //0 already, play card is the first
                                message += $"[b]{secondCard.GetBandmate().Title}[/b] steps into the limelight and plays a {secondCard.Member.GetKeyword()} card!";
                                break;
                            case 1: //1 already, revealed card is the second, is not challenge
                                message += $"[b]{bandMate.Title}[/b] is ramping it up and plays a {secondCard.Member.GetKeyword()} card!";
                                break;
                            case 2: //2 in play, but is challenge so already flipped
                                message = $"The music [b]rages[/b] and a {secondCard.Member.GetKeyword()} card is played! [b]{bandMate.Title}[/b] is going into overdrive!";
                                break;
                            default:
                                message = $"[b]{bandMate.Title}[/b] is ramping it up and plays a {secondCard.Member.GetKeyword()} card!";
                                break;
                        }
                    }
                }

                
                if (sendMessage)
                {
                    CardController cc = FindCardController(card);
                    var coroutineFlip = GameController.FlipCard(cc, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutineFlip);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutineFlip);
                    }
                    var coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { card });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutineFlip = GameController.FlipCard(secondCard, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutineFlip);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutineFlip);
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

            // add check for targets just in case
            // this should never matter since if there are no villain targets, the game should already be over
            if (highest.Any())
            {

                coroutine = GameController.DealDamage(DecisionMaker, highest.First(), c => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, 2, DamageType.Sonic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
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