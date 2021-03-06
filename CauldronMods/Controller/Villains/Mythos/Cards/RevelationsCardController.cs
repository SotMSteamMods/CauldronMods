﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class RevelationsCardController : MythosUtilityCardController
    {
        public RevelationsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //{Mythos} regains {H} HP for each environment card in play. 
            foreach (Card c in base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndHasGameText)))
            {
                coroutine = base.GameController.GainHP(base.CharacterCard, base.Game.H, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Move 2 cards from the villain trash to the bottom of the villain deck.
            List<MoveCardDestination> destinations = new List<MoveCardDestination>
            {
                new MoveCardDestination(TurnTaker.Deck, toBottom: true)
            };
            coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, 2, 2, new LinqCardCriteria(c => true), destinations, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (base.IsTopCardMatching(MythosClueDeckIdentifier))
            {
                //{MythosClue} Reduce damage dealt by hero targets by 1 until the start of the villain turn.
                ReduceDamageStatusEffect reduceDamageStatus = new ReduceDamageStatusEffect(1);
                reduceDamageStatus.SourceCriteria.IsHero = true;
                reduceDamageStatus.SourceCriteria.IsTarget = true;
                reduceDamageStatus.UntilStartOfNextTurn(base.TurnTaker);

                coroutine = base.AddStatusEffect(reduceDamageStatus);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                //{MythosMadness} Each Minion regains {H} HP.
                coroutine = base.GameController.GainHP(this.DecisionMaker, (Card c) => base.IsMinion(c), base.Game.H, cardSource: GetCardSource());
                if (UseUnityCoroutines)
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
    }
}
