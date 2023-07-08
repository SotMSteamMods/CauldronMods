using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class HostageShieldCardController : CardController
    {
        //==============================================================
        // Play this card next to the hero with the lowest HP.
        // That hero cannot deal damage.
        //
        // At the start of their turn, a hero may skip the rest of
        // their turn to destroy this card.
        //==============================================================

        public static readonly string Identifier = "HostageShield";

        public HostageShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<Card> storedHeroes = new List<Card>();
            IEnumerator routine = base.GameController.FindTargetWithLowestHitPoints(1,
                c =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 
                storedHeroes, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!storedHeroes.Any())
            {
                yield break;
            }

            routine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(c => c == storedHeroes.First()), 
                storedResults, isPutIntoPlay, decisionSources); 
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator Play()
        {
            if (!base.Card.Location.IsNextToCard)
            {
                yield break;
            }

            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect
            {
                IsPreventEffect = true,
                SourceCriteria = { IsSpecificCard = base.GetCardThisCardIsNextTo() }
            };
            cddse.UntilCardLeavesPlay(base.Card);

            IEnumerator routine = base.GameController.AddStatusEffect(cddse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => IsHero(tt), base.SkipTheirTurnToDestroyThisCardResponse, new[]
            {
                TriggerType.SkipTurn,
                TriggerType.DestroySelf
            });

            base.AddTriggers();
        }
    }
}