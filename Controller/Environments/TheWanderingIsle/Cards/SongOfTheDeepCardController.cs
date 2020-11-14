using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class SongOfTheDeepCardController : CardController
    {
        public SongOfTheDeepCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DrawCardsResponse), new TriggerType[] { TriggerType.DrawCard, TriggerType.DestroySelf }, (PhaseChangeAction pca) => this.IsTeryxInPlay(base.TurnTakerController), false);
        }

        private IEnumerator DrawCardsResponse(PhaseChangeAction pca)
        {
            //each player may draw a card. 
            IEnumerator coroutine = base.EachPlayerDrawsACard((HeroTurnTaker tt) => true, true, true, null, true, false);
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
                IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroy);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroy);
                }

            }
            yield break;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, play the top card of the environment deck.
            IEnumerator play = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 1, false, null, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
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
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsCreature(c), false, null, false).Count<Card>();
        }

        private bool IsCreature(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "creature", false, false);
        }

        private bool IsTeryxInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsTeryx(c));
            var numCardsInPlay = cardsInPlay.Count();

            return numCardsInPlay > 0;
        }
        private bool IsTeryx(Card card)
        {
            return card.Identifier == "Teryx";
        }
    }
}
