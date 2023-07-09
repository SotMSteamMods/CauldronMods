using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Menagerie
{
    public class LumobatFlockCardController : MenagerieCardController
    {
        public LumobatFlockCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        private const string FirstSpecimen = "FirstSpecimen";

        public override void AddTriggers()
        {
            //The first time a Specimen enters play each turn, play the top card of the villain deck.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => action.CardEnteringPlay != Card && !base.HasBeenSetToTrueThisTurn(FirstSpecimen) && action.IsSuccessful && base.IsSpecimen(action.CardEnteringPlay), this.PlayVillainCardResponse, new TriggerType[] { TriggerType.PlayCard }, TriggerTiming.After);

            //At the end of the villain turn this card deals the hero target with the highest HP 2 projectile and 2 radiant damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
            base.AddTriggers();

        }

        private IEnumerator PlayVillainCardResponse(CardEntersPlayAction action)
        {
            ////The first time a Specimen enters play each turn...
            base.SetCardPropertyToTrueIfRealAction(FirstSpecimen);
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
            yield break;
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the highest HP 2 projectile and 2 radiant damage.
            IEnumerator coroutine = base.DealMultipleInstancesOfDamageToHighestLowestHP(new List<DealDamageAction>()
            {
                new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Projectile),
                new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Radiant)
            }, (Card c) => IsHero(c), HighestLowestHP.HighestHP);
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