using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class AMerryChaseCardController : GyrosaurUtilityCardController
    {
        public AMerryChaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var damagedByString = SpecialStringMaker.ShowDamageTaken(CharacterCard, sinceCardEnteredPlay: this.Card, showDamageDealers: true);
            damagedByString.Condition = () => this.Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            //"Whenever a target deals damage to {Gyrosaur}, all other hero targets become immune to damage dealt by that target until this card leaves play.",
            AddTrigger((DealDamageAction dd) =>  dd.DamageSource != null && dd.DamageSource.IsTarget && dd.Target == CharacterCard && dd.DidDealDamage, MakeImmuneToDamageResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);
            //"At the start of your turn, destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        // NOTE:The message currently says "All hero targets except for Gyrosaur is...". It should be "are...". This is a limitation with the engine.
        // Handelabra needs to fix their IsSingular method to take into account "include all except" conditions and treat them as plural.
        private IEnumerator MakeImmuneToDamageResponse(DealDamageAction dd)
        {
            //All other hero targets become immune to damage dealt by that target until this card leaves play.
            var immunityEffect = new ImmuneToDamageStatusEffect();
            immunityEffect.SourceCriteria.IsSpecificCard = dd.DamageSource.Card;
            immunityEffect.TargetCriteria.IsHero = true;
            immunityEffect.TargetCriteria.IsTarget = true;
            immunityEffect.TargetCriteria.IsNotSpecificCard = CharacterCard;
            immunityEffect.TargetCriteria.OutputString = $"All hero targets except for {CharacterCard.Title}";
            immunityEffect.UntilCardLeavesPlay(Card);
            immunityEffect.UntilTargetLeavesPlay(dd.DamageSource.Card);
            immunityEffect.CardSource = Card;

            IEnumerator coroutine = AddStatusEffect(immunityEffect);
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
