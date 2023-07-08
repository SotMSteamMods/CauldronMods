using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class TheRamCharacterCardController : TheRamUtilityCharacterCardController
    {
        public TheRamCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            AddUpCloseTrackers();
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            if (TurnTaker.IsChallenge)
            {
                SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisGame(ChallengeRetreatKey), () => "The Ram has been reduced to 40 HP this game.", () => "The Ram has not yet been reduced to 40 HP.");
            }
        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            (TurnTakerController as TheRamTurnTakerController).HandleWintersEarly(true);
        }

        private readonly string ChallengeRetreatKey = "TheRamChallengeTacticalRetreatKey";

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                //front side
                //"Whenever all active heroes are Up Close, flip {TheRam}'s character cards and destroy all copies of Fall Back,",
                Func<GameAction, bool> potentialFlipTriggers = (GameAction ga) => (ga is FlipCardAction && (ga as FlipCardAction).CardToFlip != this || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && 
                                                                                        !FindActiveHeroTurnTakerControllers().Where((HeroTurnTakerController httc) => !IsUpClose(httc.TurnTaker)).Any();
                //the second criterion: "we cannot find any heroes that are not up close"
                AddSideTrigger(AddTrigger(potentialFlipTriggers, FlipToBack, TriggerType.FlipCard, TriggerTiming.After));

                //"At the start of a hero's turn, if that hero is not Up Close, you may take a copy of Up close from the villain trash and play it next to that hero.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !IsUpClose(tt) && !tt.IsIncapacitatedOrOutOfGame, AskIfMoveUpCloseResponse, TriggerType.PutIntoPlay));

                //"Reduce damage dealt to {TheRam} by 1. Increase damage dealt to {TheRam} by Up Close targets by 1.",
                AddSideTrigger(AddReduceDamageTrigger((Card c) => c == this.Card, 1));
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsCard && IsUpClose(dd.DamageSource.Card), 1));

                //"At the end of the villain turn, {TheRam} deals the hero target with the highest HP {H - 1} melee damage."
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => c.IsInPlayAndHasGameText && IsHero(c), TargetType.HighestHP, H - 1, DamageType.Melee));

                if (IsGameAdvanced)
                {
                    //"advanced": "Increase damage dealt to Up Close heroes by 1."
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target != null && IsHero(dd.Target) && IsUpClose(dd.Target), 1));
                }
            }
            else
            {
                //back side
                //"When {TheRam}'s character cards are flipped to this side, search the villain trash for Grappling claw and put it into play.",
                //handled on front

                //"If a copy of Close Up attached to a hero leaves play, flip {TheRam}'s character cards.",
                Func<MoveCardAction, bool> upCloseLeaves = (MoveCardAction mc) => mc.CardToMove != null && 
                                                                                    mc.CardToMove.Identifier == "UpClose" 
                                                                                    && mc.Origin.IsNextToCard 
                                                                                    && !mc.Destination.IsInPlay
                                                                                    && mc.WasCardMoved;
                AddSideTrigger(AddTrigger(upCloseLeaves, (MoveCardAction mc) => GameController.FlipCard(this, cardSource:GetCardSource()), TriggerType.FlipCard, TriggerTiming.After));

                //"At the end of the villain turn, {TheRam} deals the hero target with the highest HP {H - 1} melee damage."
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => c.IsInPlayAndHasGameText && IsHero(c), TargetType.HighestHP, H - 1, DamageType.Melee));

                //"Increase damage dealt to {TheRam} by hero targets by 1.",
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsTarget && IsHero(dd.DamageSource.Card), 1));

                if (IsGameAdvanced)
                {
                    //"flippedAdvanced": "Whenever a villain target is destroyed, play the top card of the villain deck.",
                    AddSideTrigger(AddTrigger((DestroyCardAction dc) => dc.CardToDestroy != null && IsVillainTarget(dc.CardToDestroy.Card), PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard, TriggerTiming.After));
                }
            }

            if(Game.IsChallenge && !HasBeenSetToTrueThisGame(ChallengeRetreatKey))
            {
                //"The first time in the game that {TheRam} is reduced to 40 or fewer HP, search the villain deck and trash for a copy of Fall Back and put it into play, then shuffle all cards other than Up Close and Grappling Claw from the villain trash into the villain deck."
                AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.Target == CharacterCard && dd.TargetHitPointsAfterBeingDealtDamage <= 40 && !dd.Target.IsBeingDestroyed && !HasBeenSetToTrueThisGame(ChallengeRetreatKey), ChallengeResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.ShuffleTrashIntoDeck }, TriggerTiming.After));
                AddSideTrigger(AddTrigger((SetHPAction sha) => sha.HpGainer == CharacterCard && sha.HpGainer.HitPoints <= 40 && !sha.HpGainer.IsBeingDestroyed && !HasBeenSetToTrueThisGame(ChallengeRetreatKey), ChallengeResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.ShuffleTrashIntoDeck }, TriggerTiming.After));

            }

            AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator ChallengeResponse(GameAction action)
        {
            SetCardPropertyToTrueIfRealAction(ChallengeRetreatKey);
            IEnumerator coroutine;
            //When {TheRam} is reduced to 40 or fewer HP, search the villain deck and trash for a copy of Fall Back and put it into play. 

            var fallBacks = TurnTaker.GetCardsWhere((Card c) => c.Identifier == "FallBack" && (c.Location == TurnTaker.Deck || c.Location == TurnTaker.Trash));
            if (fallBacks.Any())
            {
                //PlayCardFromLocations will play *one* copy from *each* location, if possible.
                //So, we'll just pick the first one we can find and play it
                //(They should both end up either in play or in the deck afterwards, so it shouldn't matter)
                coroutine = GameController.PlayCard(DecisionMaker, fallBacks.FirstOrDefault(), isPutIntoPlay: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = GameController.SendMessageAction("There were no copies of Fall Back in the villain deck or trash.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Put all cards other than UpClose and Grappling Claw from the villain trash into the villain deck, then shuffle the villain deck.
            IEnumerable<Card> cardsToMove = TurnTaker.Trash.Cards.Where(c => c.Identifier != "UpClose" && c.Identifier != "GrapplingClaw");
            coroutine = GameController.BulkMoveCards(TurnTakerController, cardsToMove, TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
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

        private IEnumerator AskIfMoveUpCloseResponse(PhaseChangeAction pc)
        {
            Card upClose = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash && c.Identifier == "UpClose"), GetCardSource()).FirstOrDefault();
            if (upClose == null)
            {
                IEnumerator message = GameController.SendMessageAction("There were no copies of Up Close in the trash to take.", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            TurnTaker activeHero = pc.ToPhase.TurnTaker;
            HeroTurnTakerController player = FindHeroTurnTakerController(activeHero.ToHero());

            List<YesNoCardDecision> storedResult = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.Custom, upClose, pc, storedResult, new Card[] { upClose, this.Card }, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidPlayerAnswerYes(storedResult))
            {
                yield break;
            }
            
            List<Card> heroResult = new List<Card> { };
            coroutine = GameController.FindCharacterCard(player, activeHero, SelectionType.HeroCharacterCard, heroResult, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            Card hero = heroResult.FirstOrDefault();

            if (hero != null)
            {
                coroutine = (FindCardController(upClose) as UpCloseCardController).PlayBySpecifiedHero(hero, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            
            yield break;
        }

        private IEnumerator FlipToBack(GameAction ga)
        {
            //"Whenever all active heroes are Up Close, flip {TheRam}'s character cards..."

            IEnumerator coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //...and destroy all copies of Fall Back"
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c != null && c.IsInPlayAndHasGameText && c.Identifier == "FallBack"), autoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //"When {TheRam}'s character cards are flipped to this side, search the villain trash for Grappling claw and put it into play.",
            coroutine = GameController.SelectAndMoveCard(DecisionMaker, (Card c) => c != null && c.Location == TurnTaker.Trash && c.Identifier == "GrapplingClaw", TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to move up close?", "Should they move up close?", "Vote for if they should move up close?", "move up close");

        }
    }
}
