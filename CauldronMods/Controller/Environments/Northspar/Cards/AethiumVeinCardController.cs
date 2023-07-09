using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class AethiumVeinCardController : NorthsparCardController
    {

        public AethiumVeinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(TakAhabIdentifier);
        }

        public override void AddTriggers()
        {
            //If this card is destroyed by a hero card and Tak Ahab is in play, place the top card of the villain deck beneath him.
            Func<DestroyCardAction, bool> criteria = (DestroyCardAction dca) => dca.WasDestroyedBy(c => IsHero(c)) && base.IsTakAhabInPlay();
            base.AddWhenDestroyedTrigger(this.WhenDestroyedResponse, new TriggerType[] { TriggerType.MoveCard }, criteria);

            //At the start of the environment turn, destroy this card and Tak Ahab's end of turn effect acts twice this turn.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.StartOfTurnResponse, new TriggerType[]
                {
                    TriggerType.DestroySelf,
                    TriggerType.Hidden
                });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //Tak Ahab's end of turn effect acts twice this turn.
            var card = base.FindTakAhabAnywhere();
            if (card != null && IsRealAction())
            {
                Journal.RecordCardProperties(card, AethiumTriggerKey, Game.TurnIndex);
            }

            // destroy this card
            IEnumerator coroutine = base.DestroyThisCardResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator WhenDestroyedResponse(DestroyCardAction dca)
        {
            //Place the top card of the villain deck beneath him.
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = base.FindVillainDeck(base.DecisionMaker, SelectionType.MoveCardToUnderCard, storedResults, (Location l) => l.HasCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.DidSelectLocation(storedResults))
            {
                Location villainDeck = storedResults.First().SelectedLocation.Location;
                Card takAhab = base.FindTakAhabInPlay();
                coroutine = base.GameController.MoveCard(base.DecisionMaker, villainDeck.TopCard, takAhab.UnderLocation, showMessage: true, cardSource: base.GetCardSource());
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
    }
}