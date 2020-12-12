using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SuperstormAkela
{
    public class ScatterburstCardController : SuperstormAkelaCardController
    {

        public ScatterburstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SetCardProperty("TurnEnteredPlay", false);
        }

        public override IEnumerator Play()
        {
            SetCardPropertyToTrueIfRealAction("TurnEnteredPlay");
            yield break;
        }

        public override void AddTriggers()
        {
            //On the turn this card enters play, after all other end of turn effects have taken place, shuffle all non-character villain cards from the villain play area and replace them in a random order. Do the same for environment cards in the environment play area.
            //Then, each hero target regains 1HP and this card is destroyed.

            //AddPhaseChangeTrigger(tt => DidEnterPlayLastTurn(), p => p == Phase.Start, _ => true, ShuffleResponse, new TriggerType[] { TriggerType.ShuffleCards, TriggerType.GainHP, TriggerType.DestroySelf }, TriggerTiming.Before);
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction p) => DidEnterPlayLastTurn() && p.FromPhase.IsEnd, ShuffleResponse, new TriggerType[] { TriggerType.ShuffleCards, TriggerType.GainHP, TriggerType.DestroySelf }, TriggerTiming.Before);

        }

        private IEnumerator ShuffleResponse(PhaseChangeAction pca)
        {
            Log.Debug("Hitting the shuffle response");
            //shuffle all non-character villain cards from the villain play area and replace them in a random order
            List<TurnTakerController> storedResults = new List<TurnTakerController>();
            IEnumerator coroutine = GameController.FindVillainTurnTakerController(DecisionMaker, SelectionType.MoveCard, storedResults, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults.Any())
            {
                TurnTakerController selectedVillainTurnTakerController = storedResults.First();
                coroutine = GameController.ShuffleCardsInPlayArea(selectedVillainTurnTakerController, new LinqCardCriteria((Card c) => !c.IsCharacter && IsVillain(c), "non-character villain card"), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Do the same for environment cards in the environment play area.
            coroutine = GameController.ShuffleCardsInPlayArea(base.TurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment card"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, each hero target regains 1HP
            coroutine = base.GameController.GainHP(DecisionMaker, (Card c) => c.IsHero && c.IsTarget, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and this card is destroyed.
            coroutine = DestroyThisCardResponse(pca);
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

        private bool DidEnterPlayLastTurn()
        {
            bool enteredPlayLastTurn = (from e in base.GameController.Game.Journal.CardPropertiesEntriesThisRound((CardPropertiesJournalEntry prop) => prop.Key == "TurnEnteredPlay" && prop.BoolValue == true)
                                                            where e.TurnIndex == Game.TurnIndex
                                                            select e).Any();
            return enteredPlayLastTurn;
        }
    }
}