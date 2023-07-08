using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class UrgentMissionCardController : NightloreCitadelUtilityCardController
    {
        public UrgentMissionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the highest HP.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredHighestHitPoints(c, (Card card) => IsHeroCharacterCard(card) && !card.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(card, GetCardSource())) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero with the highest hp"), storedResults, isPutIntoPlay, decisionSources);

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
            //Targets in this play area cannot deal damage and are immune to damage dealt by environment cards.
            AddCannotDealDamageTrigger((Card c) => c.Location == Card.Location.OwnerTurnTaker.PlayArea && c.IsTarget);
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target != null && dd.Target.Location == Card.Location.OwnerTurnTaker.PlayArea && dd.DamageSource != null && dd.DamageSource.IsEnvironmentCard);
            //At the start of the environment turn, this player draws 2 cards. Then, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, StartOfTurnResponse, new TriggerType[]
            {
                TriggerType.DrawCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            //this player draws 2 cards
            if(GetCardThisCardIsNextTo() != null && GetCardThisCardIsNextTo().IsInPlayAndHasGameText)
            {
                HeroTurnTakerController httc = FindHeroTurnTakerController(GetCardThisCardIsNextTo().Owner.ToHero());
                coroutine = GameController.SendMessageAction($"{Card.Title} causes {httc.Name} to draw 2 cards.", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DrawCards(httc, 2);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(pca);
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
