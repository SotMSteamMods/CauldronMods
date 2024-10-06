using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class ReinforcedSphereCardController : EnclosureCardController
    {
        public ReinforcedSphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, place the top card of the environment deck beneath it face down.
            IEnumerator coroutine = base.EncloseTopCardResponse(FindEnvironment().TurnTaker.Deck);
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
            //Reduce damage dealt by hero targets by 1.
            base.AddReduceDamageTrigger((DealDamageAction action) => action.DamageSource.IsHeroTarget, (DealDamageAction action) => 1);
        }
    }
}
