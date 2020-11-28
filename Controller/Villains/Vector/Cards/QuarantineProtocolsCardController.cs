using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class QuarantineProtocolsCardController : CardController
    {
        //==============================================================
        // Environment targets are immune to damage.
        // At the end of the villain turn, each player must discard a card.
        // When this card is destroyed, it deals each hero
        // target {H} lightning damage.
        //==============================================================

        public static readonly string Identifier = "QuarantineProtocols";

        private const int CardsToDiscard = 1;

        public QuarantineProtocolsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn, each player must discard a card.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, DiscardCardResponse, TriggerType.DiscardCard);

            // When this card is destroyed, it deals each hero target {H} lightning damage.
            base.AddWhenDestroyedTrigger(DestroyCardResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Environment targets are immune to damage.
            ImmuneToDamageStatusEffect itdse = new ImmuneToDamageStatusEffect { TargetCriteria = {IsEnvironment = true} };

            IEnumerator routine = base.GameController.AddStatusEffect(itdse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator DiscardCardResponse(PhaseChangeAction pca)
        {
            IEnumerator routine = base.GameController.EachPlayerDiscardsCards(1, CardsToDiscard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            int damageToDeal = base.Game.H;

            IEnumerator routine = this.DealDamage(this.Card, card => card.IsHero && card.IsTarget && card.IsInPlay, 
                damageToDeal, DamageType.Lightning);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}