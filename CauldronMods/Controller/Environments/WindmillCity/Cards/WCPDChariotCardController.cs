using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class WCPDChariotCardController : WindmillCityUtilityCardController
    {
        public WCPDChariotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(ranking: 2);
        }
        private Guid? _ownDamage;
        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP {H - 1} projectile damage.
            //If a hero target would be dealt damage this way, that hero may discard 2 cards to redirect that damage to a non-environment target.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.DiscardCard
                });
            AddTrigger((DealDamageAction dd) => IsHero(dd.Target.Owner) && dd.InstanceIdentifier == _ownDamage, RedirectHeroDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before, isConditional: true, isActionOptional: true);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            List<Card> lowestCard = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(2, (Card c) => c.IsNonEnvironmentTarget, lowestCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(lowestCard != null && lowestCard.FirstOrDefault() != null)
            {
                Card target = lowestCard.FirstOrDefault();
                DealDamageAction dd = new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), target, Game.H - 1, DamageType.Projectile);
                _ownDamage = dd.InstanceIdentifier;        
                coroutine = DoAction(dd);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                _ownDamage = null;

            }
            yield break;

        }

        private IEnumerator RedirectHeroDamageResponse(DealDamageAction dd)
        {
            HeroTurnTakerController hero = FindHeroTurnTakerController(dd.Target.Owner.ToHero());
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCards(hero, 2, true, 2, gameAction: dd, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(storedResults, numberExpected: 2))
            {
                CardSource heroCharacter;
                if (hero.HasMultipleCharacterCards)
                {
                    heroCharacter = FindCardController(hero.CharacterCards.First()).GetCardSource();
                }
                else
                {
                    heroCharacter = hero.CharacterCardController.GetCardSource();
                }
                coroutine = base.GameController.SelectTargetAndRedirectDamage(hero, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, heroCharacter), dd, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
