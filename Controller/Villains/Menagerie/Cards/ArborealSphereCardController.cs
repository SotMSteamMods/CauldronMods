using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class ArborealSphereCardController : EnclosureCardController
    {
        public ArborealSphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, place the top card of the villain deck beneath it face down. 
            IEnumerator coroutine = base.EncloseTopCardResponse();
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
            base.AddTriggers();
            //Whenever a Specimen enters play, it deals the non-villain target with the lowest HP {H - 2} melee damage.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => base.IsSpecimen(action.CardEnteringPlay) && action.IsSuccessful, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(CardEntersPlayAction action)
        {
            //...it deals the non-villain target with the lowest HP {H - 2} melee damage.
            IEnumerator coroutine = base.DealDamageToLowestHP(action.CardEnteringPlay, 1, (Card c) => !base.IsVillainTarget(c), (Card c) => Game.H - 2, DamageType.Melee);
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
    }
}