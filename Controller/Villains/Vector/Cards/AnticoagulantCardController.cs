using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class AnticoagulantCardController : CardController
    {
        //==============================================================
        // Increase damage dealt to {Vector} by 1.
        // When {Vector} is dealt damage, destroy this card and {Vector}
        // deals each hero target X toxic damage, where X is the amount
        // of damage that was dealt to {Vector}.
        //==============================================================

        public static readonly string Identifier = "Anticoagulant";

        private const int IncreaseDamageAmount = 1;

        public AnticoagulantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(IncreaseDamageAmount);
            idse.UntilCardLeavesPlay(this.Card);

            IEnumerator coroutine = base.AddStatusEffect(idse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}