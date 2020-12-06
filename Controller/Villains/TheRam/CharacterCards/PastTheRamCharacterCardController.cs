using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class PastTheRamCharacterCardController : TheRamUtilityCharacterCardController
    {
        public PastTheRamCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            AddUpCloseTrackers();
        }

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                //"If {TheRam} is destroyed remove this card from the game.",
                //"Whenever a villain one-shot would enter play, put it beneath this card instard. Cards beneath this one are not considered in play.",
                //"At the start of the villain turn, if there are 3 or more cards beneath this one, flip {TheRam}'s character cards."

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to {TheRam} by 1",
                }
            }
            else
            {
                //"If {TheRam} is destroyed remove this card from the game.",
                //"When {TheRam} flips to this side, it regains {H + 2} HP. Then, put all cards beneath this one into play in any order.",
                //"At the start of the villain turn, if {TheRam} did not flip this turn, flip {TheRam}'s villain character cards.",
                //"Increase projectile damage dealt by villain targets by 1."

                if (IsGameAdvanced)
                {
                    //"At the end of the villain turn, play the top card of the villain deck.",
                }
            }
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
