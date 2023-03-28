using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class ExoticSphereCardController : EnclosureCardController
    {
        public ExoticSphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP();
        }


        public override IEnumerator Play()
        {
            //When this card enters play, place the top 2 cards of the villain deck beneath it face down.
            IEnumerator coroutine = base.EncloseTopCardResponse(TurnTaker.Deck);
            IEnumerator coroutine2 = base.EncloseTopCardResponse(TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            //At the start of each hero's turn, this card deals the non-villain target with the highest HP {H - 1} toxic damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt), this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the non-villain target with the highest HP {H - 1} toxic damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => !base.IsVillainTarget(c), (Card c) => Game.H - 1, DamageType.Toxic);
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