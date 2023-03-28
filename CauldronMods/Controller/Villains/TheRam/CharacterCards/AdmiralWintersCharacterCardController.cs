using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace Cauldron.TheRam
{
    public class AdmiralWintersCharacterCardController : TheRamUtilityCharacterCardController
    {
        private Card ram { get { return this.CharacterCard; } }
        private const string redirectKey = "AdmiralWintersRedirectKey";
        public override bool CanBeDestroyed => false;
        public AdmiralWintersCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            AddUpCloseTrackers();
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "UpClose", "", false, false, "copy of Up Close", "copies of Up Close"));

        }

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                //"The heroes cannot win the game. If {AdmiralWinters} would be destroyed, flip his character cards instead.",
                AddSideTrigger(AddTrigger((GameOverAction ga) => ga.ResultIsVictory && ga.EndingResult != EndingResult.PrematureVictory, CancelWithMessageResponse, TriggerType.CancelAction, TriggerTiming.Before));

                //"At the start of a hero's turn, if that hero is not Up Close, you may take a copy of Up Close from the villain trash and play it next to that hero.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !IsUpClose(tt) && !tt.IsIncapacitatedOrOutOfGame, AskIfMoveUpCloseResponse, TriggerType.PutIntoPlay));

                //"{AdmiralWinters} is immune to damage from targets that are not up close. 
                AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsTarget && !IsUpClose(dd.DamageSource.Card));

                //The first time {AdmiralWinters} would be dealt damage each turn, redirect that damage to {TheRam}.",
                AddSideTriggers(AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => dd.Target == this.Card && ram.IsInPlayAndNotUnderCard, redirectKey, TargetType.HighestHP, (Card c) => c == ram));

                //"At the end of the villain turn, {AdmiralWinters} deals {H} projectile damage to each hero that is not Up Close. 
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) =>  IsHeroCharacterCard(c) && !IsUpClose(c), TargetType.All, H, DamageType.Projectile));

                //Then, play the top X cards of the villain deck, where X is the number of copies of Up Close in play minus 1."
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayCardsBasedOnUpClose, TriggerType.PlayCard));

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to {AdmiralWinters} by 1.",
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c == this.Card, 1));
                }
            }
            else
            {
                //"At the start of a hero's turn, if that hero is not Up Close, you may take a copy of Up close from the villain trash and play it next to that hero.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !IsUpClose(tt) && !tt.IsIncapacitatedOrOutOfGame, AskIfMoveUpCloseResponse, TriggerType.PutIntoPlay));

                //"Increase damage dealt to and by {TheRam} by 1.",
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card != null && dda.DamageSource.Card == ram, 1));
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dda) => dda.Target == ram, 1));

                //"Whenever a one-shot is placed under {TheRam}'s character cards, immediately flip {TheRam}'s character cards."
                AddSideTrigger(AddTrigger<MoveCardAction>(ImmediateFlipRamCriteria, FlipRamResponse, TriggerType.FlipCard, TriggerTiming.After));
                AddSideTrigger(AddTrigger((BulkMoveCardsAction bmc) => bmc.Destination == ram.UnderLocation && bmc.CardsToMove.Any((Card c) => c.IsOneShot) && bmc.CardsToMove.Any((Card c) => c.Location == ram.UnderLocation && c.DoKeywordsContain("one-shot", true)), FlipRamResponse, TriggerType.FlipCard, TriggerTiming.After));

                AddSideTrigger(AddCannotDealDamageTrigger((Card c) => c == this.Card));

                AddSideTrigger(AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card == this.CharacterCardWithoutReplacements && !AskIfCardIsIndestructible(dc.CardToDestroy.Card), DefeatedResponse, TriggerType.GameOver, TriggerTiming.Before));

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to villain targets by 1.",
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => IsVillainTarget(c), 1));
                }
            }
            AddDefeatedIfMovedOutOfGameTriggers();
        }

        private bool ImmediateFlipRamCriteria(MoveCardAction mc)
        {
            return mc.Destination == ram.UnderLocation && mc.CardToMove.DoKeywordsContain("one-shot", true) && mc.CardToMove.Location == ram.UnderLocation;
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            CardSource cardSource = flip.CardSource;
            if (cardSource == null && flip.ActionSource != null)
            {
                cardSource = flip.ActionSource.CardSource;
            }
            if (cardSource == null)
            {
                cardSource = GetCardSource();
            }
            IEnumerator coroutine = base.GameController.RemoveTarget(base.Card, leavesPlayIfInPlay: true, cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PlayCardsBasedOnUpClose(GameAction ga)
        {
            //I think this wants to be dynamic in case Up Closes get played or destroyed in the middle of things
            Func<int> numUpClose = () => FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "UpClose" && c.IsInPlayAndHasGameText)).Count();
            IEnumerator coroutine;
            for(int i = 1; i <= numUpClose() - 1; i++)
            {
                string ordinal = "a";
                switch (i)
                { 
                    case 2:
                        {
                            ordinal = "a second";
                            break;
                        }
                    case 3:
                        {
                            ordinal = "a third";
                            break;
                        }
                    case 4:
                        {
                            ordinal = "a fourth";
                            break;
                        }
                    //not possible to get to 5
                }
                coroutine = GameController.SendMessageAction($"Admiral Winters plays {ordinal} card...", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = PlayTheTopCardOfTheVillainDeckResponse(ga);
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

        private IEnumerator CancelWithMessageResponse(GameOverAction go)
        {
            IEnumerator coroutine;
            if (!HasBeenSetToTrueThisGame("HeroesCannotWinMessage"))
            {
                coroutine = base.GameController.SendMessageAction($"The heroes cannot win the game while {Card.Title} is active!", Priority.Critical, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                SetCardPropertyToTrueIfRealAction("HeroesCannotWinMessage");
            }
            coroutine = CancelAction(go);
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
        private IEnumerator FlipRamResponse(GameAction ga)
        {
            IEnumerator coroutine = GameController.SendMessageAction("Admiral Winters immediately flips the Ram!", Priority.High, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.FlipCard(CharacterCardController, cardSource: GetCardSource());
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

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            //If {AdmiralWinters} would be destroyed, flip his character cards instead.
            if (!this.Card.IsFlipped)
            {
                IEnumerator coroutine = GameController.FlipCard(this, cardSource: GetCardSource(), allowBackToFront: false);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = CheckForVictory(destroyCard);
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

        private IEnumerator CheckForVictory(GameAction ga)
        {
            if (this.Card.IsFlipped && (!this.CharacterCardWithoutReplacements.IsInPlay || this.CharacterCardWithoutReplacements.IsBeingDestroyed))
            {
                IEnumerator coroutine = DefeatedResponse(ga);
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

        private IEnumerator AskIfMoveUpCloseResponse(PhaseChangeAction pc)
        {
            Card upClose = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash && c.Identifier == "UpClose")).FirstOrDefault();
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to move up close?", "Should they move up close?", "Vote for if they should move up close?", "move up close");

        }
    }
}
