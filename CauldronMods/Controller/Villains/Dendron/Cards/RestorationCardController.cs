using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class RestorationCardController : DendronBaseCardController
    {
        //==============================================================
        // {Dendron} regains 10 HP. Restore all Tattoos to their max HP.
        //==============================================================

        public static readonly string Identifier = "Restoration";

        private const int HpToGain = 10;

        public RestorationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Dendron regains 10 HP
            IEnumerator gainHpRoutine = this.GameController.GainHP(this.HeroTurnTakerController, c => c == CharacterCard, HpToGain, cardSource: this.GetCardSource());

            // Restore all Tattoos to their max HP
            IEnumerator restoreTattooHpRoutine = this.GameController.SetHP(this.HeroTurnTakerController, IsTattoo, card => card.MaximumHitPoints.Value, cardSource: base.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(gainHpRoutine);
                yield return this.GameController.StartCoroutine(restoreTattooHpRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(gainHpRoutine);
                this.GameController.ExhaustCoroutine(restoreTattooHpRoutine);
            }
        }
    }
}