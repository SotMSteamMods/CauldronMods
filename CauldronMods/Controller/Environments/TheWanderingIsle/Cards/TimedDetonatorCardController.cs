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
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageAndDestroyResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction pca)
        {
            var teryx = base.FindTeryx();
            if (teryx != null)
            {
                //this card deals Teryx 10 fire damage
                IEnumerator teryxDamage = base.DealDamage(base.Card, (Card c) => c == teryx, 10, DamageType.Fire);
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
            IEnumerator heroDamage = base.DealDamage(base.Card, (Card c) => c.IsTarget && IsHero(c) && c.IsInPlayAndHasGameText, base.H - 2, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroDamage);
            }

            //this card is destroyed
            IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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
            IEnumerator play = base.GameController.PlayTopCard(this.DecisionMaker, base.FindEnvironment(), cardSource: base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(play);
            }
            else
            {
                base.GameController.ExhaustCoroutine(play);
            }
        }
    }
}
