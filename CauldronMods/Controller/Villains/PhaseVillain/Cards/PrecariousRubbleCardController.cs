using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class PrecariousRubbleCardController : PhaseVillainCardController
    {
        public PrecariousRubbleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the hero target with the lowest HP {H - 1} projectile damage. 
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => Game.H - 1, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then, play the top card of the villain deck.
            coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //{Phase} is immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard);
        }
    }
}