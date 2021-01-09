using System;
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
            //{Mythos} regains {H} HP for each environment card in play. Move 2 cards from the villain trash to the bottom of the villain deck.
            foreach (Card c in base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndHasGameText)))
            {
                coroutine = base.GameController.GainHP(base.CharacterCard, base.Game.H);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.IsTopCardMatching(MythosEyeDeckIdentifier))
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
            if (base.IsTopCardMatching(MythosMindDeckIdentifier))
            {
                //{MythosMadness} Each Minion regains {H} HP.
                coroutine = base.GameController.GainHP(this.DecisionMaker, (Card c) => base.IsMinion(c), base.Game.H);
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
