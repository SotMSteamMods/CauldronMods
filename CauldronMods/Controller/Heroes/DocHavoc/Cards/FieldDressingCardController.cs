using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    //==============================================================
    // Each hero target regains 1HP. You may play a card now.
    //==============================================================

    public class FieldDressingCardController : CardController
    {
        public static readonly string Identifier = "FieldDressing";
        private const int HpGain = 1;

        public FieldDressingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //==============================================================
            // Each hero target regains 1 HP.
            //==============================================================

            IEnumerator gainHpRoutine = this.GameController.GainHP(this.HeroTurnTakerController, (Func<Card, bool>)(c => IsHeroTarget(c) && c.IsInPlayAndHasGameText), HpGain,
                cardSource: this.GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return gainHpRoutine;
            }
            else
            {
                base.GameController.ExhaustCoroutine(gainHpRoutine);
            }

            //==============================================================
            // You may play a card now.
            //==============================================================

            IEnumerator playCardFromHandRoutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardFromHandRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardFromHandRoutine);
            }
        }
    }
}
