using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class StimShotCardController : CardController
    {
        //==============================================================
        // Draw a card. One player may use a power now.
        //==============================================================

        public static string Identifier = "StimShot";

        private const int CardDrawCount = 1;

        public StimShotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator drawCardsRoutine = this.DrawCards(this.HeroTurnTakerController, CardDrawCount);

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(drawCardsRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(drawCardsRoutine);
            }

            IEnumerator usePowerRoutine = this.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(usePowerRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(usePowerRoutine);
            }
        }
    }
}
