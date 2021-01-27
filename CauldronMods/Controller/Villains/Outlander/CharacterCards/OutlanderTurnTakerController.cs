using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Outlander
{
    public class OutlanderTurnTakerController : TurnTakerController
    {
        public OutlanderTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public Card DebugTraceToPlay { get; set; }

        public override IEnumerator StartGame()
        {
            //Search the villain deck for all Trace cards and put them beneath this card. 
            IEnumerator coroutine = base.GameController.MoveCards(this, base.FindCardsWhere((Card c) => this.IsTrace(c) && c.Location.IsDeck && c.IsVillain), base.CharacterCard.UnderLocation);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);

                Card traceToPlay = base.CharacterCard.UnderLocation.Cards.TakeRandomFirstOrDefault(base.GameController.Game.RNG);
                if (DebugTraceToPlay != null)
                {
                    traceToPlay = DebugTraceToPlay;
                    Log.Debug("Forcing starting Trace to be: " + traceToPlay.Title);
                }
                //Put 1 random Trace card from beneath this one into play.
                coroutine = base.GameController.PlayCard(this, traceToPlay, true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Shuffle the villain deck.
                coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck);
                if (UseUnityCoroutines)
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

        private bool IsTrace(Card c)
        {
            return c.DoKeywordsContain("trace");
        }
    }
}