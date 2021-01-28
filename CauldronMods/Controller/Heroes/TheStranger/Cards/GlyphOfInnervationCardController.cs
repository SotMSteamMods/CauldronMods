using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class GlyphOfInnervationCardController : GlyphCardController
    {
        #region Constructors

        public GlyphOfInnervationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator UsePower(int index = 0)
        {
            //Draw a card.
            IEnumerator coroutine = base.DrawCard(null, false, null, true);
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
        #endregion Methods
    }
}