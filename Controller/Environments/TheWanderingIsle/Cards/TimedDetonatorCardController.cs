using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class TimedDetonatorCardController : TheWanderingIsleCardController
    {
        public TimedDetonatorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals Teryx 10 fire damage and each hero target {H - 2} fire damage. Then, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageAndDestroyResponse), new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            }, null, false);
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction pca)
        {
            if (this.IsCardInPlay(base.TurnTakerController, "Teryx"))
            {
                //this card deals Teryx 10 fire damage
                IEnumerator teryxDamage = base.DealDamage(base.Card, (Card c) => c.IsInPlay && c.Identifier == "Teryx", 10, DamageType.Fire);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(teryxDamage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(teryxDamage);
                }
            }

            //deals each hero target {H - 2} fire damage
            IEnumerator heroDamage = base.DealDamage(base.Card, (Card c) => c.IsTarget && c.IsHero, base.H - 2, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroDamage);
            }

            //this card is destroyed
            IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroy);
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
        }

        private bool IsCardInPlay(TurnTakerController ttc, string identifier)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsCard(c, identifier));
            var numCardsInPlay = cardsInPlay.Count();

            return numCardsInPlay > 0;
        }
        private bool IsCard(Card card, string identifier)
        {
            return card.Identifier == identifier;
        }
    }
}
