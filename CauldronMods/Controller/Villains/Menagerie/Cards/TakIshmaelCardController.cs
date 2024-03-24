using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class TakIshmaelCardController : MenagerieCardController
    {
        public TakIshmaelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsEnclosure(c), "enclosure"));
        }

        public override void AddTriggers()
        {
            //This card is immune to damage dealt by non-hero cards.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == base.Card && !action.DamageSource.IsHero);

            //At the end of the villain turn, play the top card of the villain deck. Then, this card deals the hero target with the highest HP X projectile damage, where X is the number of Specimens in play.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.PlayTopCardAndDealDamageResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.DealDamage });
            base.AddTriggers();

        }

        private IEnumerator PlayTopCardAndDealDamageResponse(PhaseChangeAction action)
        {
            //...play the top card of the villain deck.
            IEnumerator coroutine = base.PlayTheTopCardOfTheVillainDeckWithMessageResponse(action);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, this card deals the hero target with the highest HP X projectile damage, where X is the number of Specimens in play.
            coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => base.FindCardsWhere(new LinqCardCriteria((Card spec) => base.IsSpecimen(spec) && spec.IsInPlayAndHasGameText)).Count(), DamageType.Projectile);
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