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
            while (targetCount + traitCount + pursuitCount != 0)
            {
                Func<Card, bool> revealCriteria = null;
                //{H - 1} targets,
                if (targetCount != 0)
                {
                    revealCriteria = (Card c) => c.IsTarget;
                }
                //2 traits, 
                if (traitCount != 0)
                {
                    if (revealCriteria == null)
                    {
                        revealCriteria = (Card c) => c.DoKeywordsContain("trait");
                    }
                    else
                    {
                        revealCriteria = (Card c) => c.DoKeywordsContain("trait") || revealCriteria(c);
                    }
                }
                //and Single-Minded Pursuit are revealed. 
                if (pursuitCount != 0)
                {
                    if (revealCriteria == null)
                    {
                        revealCriteria = (Card c) => c.Identifier == "SingleMindedPursuit";
                    }
                    else
                    {
                        revealCriteria = (Card c) => c.Identifier == "SingleMindedPursuit" || revealCriteria(c);
                    }
                }
                List<Card> playedCard = new List<Card>();
                //Put them into play, and shuffle the rest of the revealed cards back into the villain deck.
                IEnumerator coroutine = base.CharacterCardController.RevealCards_MoveMatching_ReturnNonMatchingCards(this, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria(revealCriteria), 1, storedPlayResults: playedCard, shuffleSourceAfterwards: false);
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
            yield break;
        }
    }
}