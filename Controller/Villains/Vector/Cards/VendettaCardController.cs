
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VendettaCardController : CardController
    {
        //==============================================================
        // {Vector} deals the hero target with the highest HP {H - 1} psychic damage.
        // That hero must discard a card.
        //==============================================================

        public static readonly string Identifier = "Vendetta";

        private const int CardsToDiscard = 1;

        public VendettaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator routine = base.GameController.FindTargetWithHighestHitPoints(1, c => c.IsHero 
                && !c.IsIncapacitatedOrOutOfGame, storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!storedResults.Any())
            {
                yield break;
            }

            int damageToDeal = Game.H - 1;
            routine = this.DealDamage(this.CharacterCard, storedResults.First(), damageToDeal, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Affected hero must discard a card
            routine = base.GameController.SelectAndDiscardCards(base.FindHeroTurnTakerController(storedResults.First().Owner.ToHero()), 
                CardsToDiscard, false, CardsToDiscard);

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