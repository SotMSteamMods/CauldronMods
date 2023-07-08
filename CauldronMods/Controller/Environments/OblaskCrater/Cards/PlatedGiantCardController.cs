using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class PlatedGiantCardController : OblaskCraterUtilityCardController
    {
        /* 
         * Play this card next to a hero. The hero next to this card is immune to damage from enviroment targets 
         * other than this one. 
         * At the end of the environment turn, this card deals the hero next to it {H - 1} melee damage.
         */
        public PlatedGiantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, true, decisionSources);
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

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => GetCardThisCardIsNextTo() != null && GetCardThisCardIsNextTo() == c, TargetType.All, base.H - 1, DamageType.Melee);
            base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsEnvironmentCard && dd.DamageSource.Card != Card &&  GetCardThisCardIsNextTo() != null && dd.Target == GetCardThisCardIsNextTo());
        }
        
    }
}
