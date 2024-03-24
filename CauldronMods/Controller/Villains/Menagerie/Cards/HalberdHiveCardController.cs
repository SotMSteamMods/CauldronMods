using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class HalberdHiveCardController : MenagerieCardController
    {
        public HalberdHiveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsInsect(c), "insect"));
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by insects by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && base.IsInsect(action.DamageSource.Card), 1);

            //At the end of the villain turn, this card deals the hero target with the lowest HP 2 toxic damage. Then, put all Insects in the villain trash into play.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageAndInsectResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.PutIntoPlay });
            base.AddTriggers();

        }

        private IEnumerator DealDamageAndInsectResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the lowest HP 2 toxic damage. 
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, put all Insects in the villain trash into play.
            coroutine = base.PlayCardsFromLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => base.IsInsect(c), "insect"), true);
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