using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class GlyphOfCombustionCardController : GlyphCardController
    {
        public GlyphOfCombustionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Once during your turn when {TheStranger} would deal himself damage, prevent that damage.
            base.AddTriggers();
            //Whenever a Rune or Glyph is destroyed, {TheStranger} may deal 1 target 1 fire damage.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction destroyCard) => (IsRune(destroyCard.CardToDestroy.Card) || IsGlyph(destroyCard.CardToDestroy.Card)) && destroyCard.WasCardDestroyed, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction destroyCard)
        {
            //{TheStranger} may deal 1 target 1 fire damage.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Fire, 1, false, 0,
                                        cardSource: GetCardSource());
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