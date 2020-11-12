using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class GlyphOfDecayCardController : GlyphCardController
    {
        #region Constructors

        public GlyphOfDecayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator UsePower(int index = 0)
        {
            //You may play a Rune.
            IEnumerator coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, new LinqCardCriteria((Card c) => base.IsRune(c), "rune"), false, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //TheStranger deals 1 target 1 toxic damage.
            IEnumerator coroutine2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), 1, DamageType.Toxic, new int?(1),false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
        #endregion Methods
    }
}