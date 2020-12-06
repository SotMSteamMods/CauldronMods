using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class AdmiralWintersCharacterCardController : TheRamUtilityCharacterCardController
    {
        public AdmiralWintersCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            AddUpCloseTrackers();
        }

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                //"The heroes cannot win the game. If {AdmiralWinters} would be destroyed, flip his character cards instead.",
                //"At the start of a hero's turn, if that hero is not Up Close, you may take a copy of Up Close from the villain trash and play it next to that hero.",
                //"{AdmiralWinters} is immune to damage from targets that are not up close. The first time {AdmiralWinters} would be dealt damage each turn, redirect that damage to {TheRam}.",
                //"At the end of the villain turn, {AdmiralWinters} deals {H} projectile damage to each hero that is not Up Close. Then, play the top X cards of the villain deck, where X is the number of copies of Up Close in play minus 1."

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to {AdmiralWinters} by 1.",
                }
            }
            else
            {
                //"At the start of a hero's turn, if that hero is not Up Close, you may take a copy of Up close from the villain trash and play it next to that hero.",
                //"Increase damage dealt to and by {TheRam} by 1.",
                //"Whenever a one-shot is placed under {TheRam}'s character cards, immediately flip {TheRam}'s character cards."

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to villain targets by 1.",
                }
            }

            //AddDefeatedIfDestroyedTriggers();
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
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.PutIntoPlay, upClose, pc, storedResult, new Card[] { this.Card }, GetCardSource());
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
    }
}
