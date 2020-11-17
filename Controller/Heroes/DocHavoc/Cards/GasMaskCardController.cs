using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class GasMaskCardController : CardController
    {
        public static string Identifier = "GasMask";
        private const int HpGain = 2;

        public GasMaskCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //==============================================================
            // Whenever an environment card is destroyed, {DocHavoc} regains 2 HP.
            //==============================================================

            Func<DestroyCardAction, bool> criteria = (destroyCard =>
                    destroyCard.WasCardDestroyed
                    && destroyCard.CardToDestroy != null
                    && destroyCard.CardToDestroy.Card.IsEnvironment
                    && this.GameController.IsCardVisibleToCardSource(destroyCard.CardToDestroy.Card,
                        this.GetCardSource()));
            this.AddTrigger<DestroyCardAction>(criteria, new Func<DestroyCardAction, IEnumerator>(this.GainHpResponse), TriggerType.GainHP, TriggerTiming.After);
        }

        private IEnumerator GainHpResponse(DestroyCardAction destroyCard)
        {
            IEnumerator gainHpRoutine =
                this.GameController.GainHP(this.CharacterCard, HpGain, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines)
            {

                yield return this.GameController.StartCoroutine(gainHpRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(gainHpRoutine);
            }

        }
    }
}
