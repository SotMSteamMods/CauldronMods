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
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            int damageToDeal = Game.H - 1;

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator routine = this.DealDamageToHighestHP(base.CharacterCard, 1, c => IsHeroTarget(c) && !c.IsIncapacitatedOrOutOfGame, c => damageToDeal, DamageType.Psychic, storedResults: storedResults, selectTargetEvenIfCannotDealDamage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (storedResults.FirstOrDefault() != null)
            {
                Card target = storedResults.First().OriginalTarget;
                
                // Affected hero must discard a card
                routine = base.GameController.SelectAndDiscardCards(base.FindHeroTurnTakerController(target.Owner.ToHero()),
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

            yield break;
        }
           
    }
}