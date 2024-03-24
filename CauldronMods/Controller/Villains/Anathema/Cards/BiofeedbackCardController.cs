using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cauldron.Anathema
{
    public class BiofeedbackCardController : AnathemaCardController
    {
        public BiofeedbackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //Whenever Anathema deals damage to a Hero target, he regains 1 HP.
            Func<DealDamageAction, bool> damageCriteria = (DealDamageAction dd) => dd.DidDealDamage && IsHero(dd.Target) && dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard);
            base.AddTrigger<DealDamageAction>(damageCriteria, this.DealDamageResponse, TriggerType.GainHP, TriggerTiming.After);

            //Whenever an arm, body, or head is destroyed by a Hero target, Anathema deals himself 2 psychic damage.
            Func<DestroyCardAction, bool> destroyCriteria = (DestroyCardAction dca) => dca.CardToDestroy != null && base.IsArmHeadOrBody(dca.CardToDestroy.Card) && dca.WasDestroyedBy(c => IsHeroTarget(c));
            base.AddTrigger<DestroyCardAction>(destroyCriteria, this.DestroyCardResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            //Anathema deals himself 2 psychic damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, cardSource: base.GetCardSource());
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

        private IEnumerator DealDamageResponse(DealDamageAction dd)
        {
            //he regains 1 HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(1), cardSource: base.GetCardSource());
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
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return base.TurnTaker.IsChallenge && card == base.Card && base.FindCardController(base.CharacterCard) is AnathemaCharacterCardController;
        }
    }
}
