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
                bool revealedCardsHadSameKeyword = false;
                var firstCC = FindCardController(card) as ScreaMachineBandCardController;
               
                Card cardToPlay = card;
                IEnumerator coroutine;
                ScreaMachineBandCardController cardToPlayController;
                if (!sharesAKeyword)
                {
                    // display flavor message, then flip face down
                    coroutine = GameController.MoveCard(TurnTakerController, card, Card.UnderLocation, toBottom: true, playCardIfMovingToPlayArea: false, flipFaceDown: false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    cardToPlay = this.Card.UnderLocation.TopCard;
                    cardToPlayController = FindCardController(cardToPlay) as ScreaMachineBandCardController;
                    revealedCardsHadSameKeyword = cardToPlayController.Member == firstCC.Member;

                    if (firstCC.IsBandmateInPlay && !revealedCardsHadSameKeyword)
                    {
                        var message = $"{firstCC.GetBandmate().Title} is keeping it mellow since there aren't any {firstCC.Member.GetKeyword()} cards in play!";
                        var coroutine2 = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { firstCC.Card });
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                    }

                    var coroutineFlip = GameController.FlipCard(firstCC, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutineFlip);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutineFlip);
                    }
                }
                cardToPlayController = FindCardController(cardToPlay) as ScreaMachineBandCardController;

                // flip first to display card along with flavor text
                var cc = FindCardController(cardToPlay);
                if (cardToPlay.IsFlipped)
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

                coroutine = TheSetListFlavorMessage(revealedCardsHadSameKeyword, cardToPlayController);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                List<bool> wasCardPlayed = new List<bool>();
                coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, reassignPlayIndex: true, evenIfAlreadyInPlay: true, cardSource: GetCardSource(), wasCardPlayed: wasCardPlayed);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (!wasCardPlayed.Where(b => b).Any())
                {
                    // move it back to the top of the stack face down
                    coroutine = GameController.MoveCard(TurnTakerController, cardToPlay, Card.UnderLocation, toBottom: false, playCardIfMovingToPlayArea: false, flipFaceDown: true, cardSource: GetCardSource());
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
        }
        private int ExistingCardsNeededToFlip => Game.IsChallenge ? 1 : 2;
        private IEnumerator TheSetListFlavorMessage(bool revealedCardsHadSameKeyword, ScreaMachineBandCardController cc)
        {
            if (cc.IsBandmateInPlay)
            {
                string message;
                var bandMate = cc.GetBandmate();
                int count = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain(cc.Member.GetKeyword())).Count();
                if (revealedCardsHadSameKeyword && count == 0)
                {
                    //revealed card bandmate doesn't have cards already in play, but the played card will start things off
                    message = $"[b]{bandMate.Title}[/b] is starting to feel it and plays a {cc.Member.GetKeyword()} card!";
                }
                else if (count == ExistingCardsNeededToFlip)
                {
                    //we have reached the number needed to flip
                    message = $"The music [b]surges[/b] and a {cc.Member.GetKeyword()} card is played! This is [b]{bandMate.Title}[/b] moment!";
                }
                else
                {
                    switch (count)
                    {
                        case 0: //0 already, play card is the first
                            message = $"[b]{cc.GetBandmate().Title}[/b] steps into the limelight and plays a {cc.Member.GetKeyword()} card!";
                            break;
                        case 1: //1 already, revealed card is the second, is not challenge
                            message = $"[b]{bandMate.Title}[/b] is ramping it up and plays a {cc.Member.GetKeyword()} card!";
                            break;
                        case 2: //2 in play, but is challenge so already flipped
                            message = $"The music [b]rages[/b] and a {cc.Member.GetKeyword()} card is played! [b]{bandMate.Title}[/b] is going into overdrive!";
                            break;
                        default:
                            message = $"[b]{bandMate.Title}[/b] is ramping it up and plays a {cc.Member.GetKeyword()} card!";
                            break;
                    }
                }
                var coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { cc.Card });
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
            var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), false, cardSource: GetCardSource());
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

                coroutine = GameController.DealDamage(DecisionMaker, highest.First(), c =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame, 2, DamageType.Sonic, cardSource: GetCardSource());
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