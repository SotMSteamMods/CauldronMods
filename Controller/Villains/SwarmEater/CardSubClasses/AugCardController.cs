using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class AugCardController : CardController
    {
        public AugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator ActivateAbility(string abilityKey = "absorb")
        {
            IEnumerator coroutine = this.ActivateAbsorb();
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

        public virtual IEnumerator ActivateAbsorb(Card cardThisIsUnder)
        {
            yield return null;
            yield break;
        }
    }
}