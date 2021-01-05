using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class SongOfTheDeepCardController : TheWanderingIsleCardController
    {
        public SongOfTheDeepCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("Teryx");
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsCreature(c), "creature"));
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DrawCardsResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.DestroySelf }, (PhaseChangeAction pca) => base.FindTeryx() != null);
        }

        private IEnumerator DrawCardsResponse(PhaseChangeAction pca)
        {
            //each player may draw a card. 
            IEnumerator coroutine = base.EachPlayerDrawsACard(optional: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, if there are at least 2 creatures in play, destroy this card.
            if (this.GetNumberOfCreaturesInPlay() >= 2)
            {
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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

        public override IEnumerator Play()
        {
            //When this card enters play, play the top card of the environment deck.
            IEnumerator play = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(play);
            }
            else
            {
                base.GameController.ExhaustCoroutine(play);
            }

            yield break;
        }

        private int GetNumberOfCreaturesInPlay()
        {
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsCreature(c)).Count();
        }
    }
}
