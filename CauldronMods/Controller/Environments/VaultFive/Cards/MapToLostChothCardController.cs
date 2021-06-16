using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class MapToLostChothCardController : ArtifactCardController
    {
        public MapToLostChothCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UniqueOnPlayEffect()
        {
            int numTurnTakersInBZ = GetNumberOfTurnTakersInSameBattleZone();
            List<TurnTaker> usedTurnTakers = new List<TurnTaker>();
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine;
            if (numTurnTakersInBZ > 0)
            {
                //1 player discards a card...
                coroutine = base.GameController.SelectHeroToDiscardCard(DecisionMaker, optionalDiscardCard: false, storedResultsTurnTaker: storedResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                } else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidSelectTurnTaker(storedResults))
                {
                    usedTurnTakers.Add(GetSelectedTurnTaker(storedResults));
                }
            } else
            {
                //send message saying there are no players available
                coroutine = GameController.SendMessageAction("There are no valid players to discard a card.", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            numTurnTakersInBZ = GetNumberOfTurnTakersInSameBattleZone(usedTurnTakers);
            if(numTurnTakersInBZ > 0)
            {
                storedResults = new List<SelectTurnTakerDecision>();
                coroutine = SelectHeroToPlayCard(DecisionMaker, optionalPlayCard: false, storedResultsTurnTaker: storedResults, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !usedTurnTakers.Contains(tt)));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectTurnTaker(storedResults))
                {
                    usedTurnTakers.Add(GetSelectedTurnTaker(storedResults));
                }
            } else
            {
                //send message saying there are no players available
                coroutine = GameController.SendMessageAction("There are no valid players to play a card.", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            numTurnTakersInBZ = GetNumberOfTurnTakersInSameBattleZone(usedTurnTakers);
            if(numTurnTakersInBZ > 0)
            {
                //... and a third player’s hero uses a power
                coroutine = GameController.SelectHeroToUsePower(DecisionMaker, optionalUsePower: false, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !usedTurnTakers.Contains(tt)), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else
            {
                //send message saying there are no players available
                coroutine = GameController.SendMessageAction("There are no valid players to use a power.", Priority.Medium, GetCardSource(), showCardSource: true);
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
}
