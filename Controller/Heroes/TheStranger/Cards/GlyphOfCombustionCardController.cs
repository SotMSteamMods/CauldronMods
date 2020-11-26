using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class GlyphOfCombustionCardController : GlyphCardController
    {
        #region Constructors

        public GlyphOfCombustionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //Once during your turn when {TheStranger} would deal himself damage, prevent that damage.
            base.AddTriggers();
            //Whenever a Rune or Glyph is destroyed, {TheStranger} may deal 1 target 1 fire damage.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction destroyCard) => (base.IsRune(destroyCard.CardToDestroy.Card) || base.IsGlyph(destroyCard.CardToDestroy.Card)) && destroyCard.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true,  null, false, null, null, false, false);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction destroyCard)
        {
            //{TheStranger} may deal 1 target 1 fire damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Fire, new int?(1), true, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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