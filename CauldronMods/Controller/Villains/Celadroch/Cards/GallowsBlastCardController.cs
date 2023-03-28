using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class GallowsBlastCardController : CardController
    {
        /*
         * "{Celadroch} deals each hero 5 infernal damage. Each player discards a card.",
		   "Reduce damage dealt to villain targets by 2 until the start of the villain turn."
         */

        public GallowsBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, c =>  IsHeroCharacterCard(c), 5, DamageType.Infernal, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var effect = new ReduceDamageStatusEffect(2);
            effect.TargetCriteria.IsVillain = true;
            effect.TargetCriteria.IsTarget = true;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.CardSource = Card;

            coroutine = AddStatusEffect(effect);
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