using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class CriticalMassCardController : CardController
    {
        public CriticalMassCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        //Search the villain deck and trash for all copies of Chain Reaction and put them into play. Move 1 copy of Unstable Isotope from the villain trash to the villain deck. Shuffle the villain deck.
        public override IEnumerator Play()
        {
            //{Gray} deals himself 2 energy damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Energy, cardSource: base.GetCardSource());
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