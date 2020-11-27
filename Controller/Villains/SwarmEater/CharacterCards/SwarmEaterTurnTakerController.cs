using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SwarmEater
{
    public class SwarmEaterTurnTakerController : TurnTakerController
    {
        public SwarmEaterTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Reveal cards from the top of the villain deck until {H - 1} targets, 
            int targetCount = base.H - 1;
            //2 traits, 
            int traitCount = 2;
            //and Single-Minded Pursuit are revealed. 
            int pursuitCount = 1;
            IEnumerator coroutine = null;
            while (targetCount + traitCount + pursuitCount != 0)
            {
                //{H - 1} targets,
                Func<Card, bool> targetCriteria = (Card c) => false;
                if (targetCount != 0)
                {
                    targetCriteria = (Card c) => c.IsTarget;
                }
                //2 traits, 
                Func<Card, bool> traitCriteria = (Card c) => false;
                if (traitCount != 0)
                {
                    traitCriteria = (Card c) => c.DoKeywordsContain("trait");
                }
                //and Single-Minded Pursuit are revealed. 
                Func<Card, bool> pursuitCriteria = (Card c) => false;
                if (pursuitCount != 0)
                {
                    pursuitCriteria = (Card c) => c.Identifier == "SingleMindedPursuit";
                }
                Func<Card, bool> revealCriteria = (Card c) => c != null && (targetCriteria(c) || traitCriteria(c) || pursuitCriteria(c));
                List<Card> playedCard = new List<Card>();
                //Put them into play, and shuffle the rest of the revealed cards back into the villain deck.
                coroutine = base.CharacterCardController.RevealCards_MoveMatching_ReturnNonMatchingCards(this, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria(revealCriteria), 1, storedPlayResults: playedCard, shuffleSourceAfterwards: false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (playedCard != null)
                {
                    Card card = playedCard[0];
                    if (card.IsTarget)
                    {
                        targetCount--;
                    }
                    else if (card.DoKeywordsContain("trait"))
                    {
                        traitCount--;
                    }
                    else if (card.Identifier == "SingleMindedPursuit")
                    {
                        pursuitCount--;
                    }
                }
            }
            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck);
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