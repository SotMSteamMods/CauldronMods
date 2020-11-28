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

        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<Card> storedHeroes = new List<Card>();
            IEnumerator routine = base.GameController.FindTargetWithLowestHitPoints(1,
                c => c.IsHero && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 
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
                storedResults, true, decisionSources); 
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect
            {
                IsPreventEffect = true, 
                SourceCriteria = { IsSpecificCard = storedHeroes.First() }
            };

            routine = base.GameController.AddStatusEffect(cddse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            base.AddStartOfTurnTrigger(tt => tt == storedHeroes.First().Owner, base.SkipTheirTurnToDestroyThisCardResponse, new[]
            {
                TriggerType.SkipTurn,
                TriggerType.DestroySelf
            });

            //base.AddStartOfTurnTrigger(tt => tt == storedHeroes.First().Owner, SkipTurnResponse, TriggerType.SkipTurn);
        }

        private IEnumerator SkipTurnResponse(PhaseChangeAction pca)
        {
            List<YesNoCardDecision> storedDecisions = new List<YesNoCardDecision>();
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.DecisionMaker, SelectionType.SkipTurn, this.Card,
                storedResults: storedDecisions, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!base.DidPlayerAnswerYes(storedDecisions))
            {
                yield break;
            }


            routine = base.SkipTheirTurnToDestroyThisCardResponse(pca);

            //IEnumerator routine2 = base.GameController.DestroyCard(pca.DecisionMaker, this.Card, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
                //yield return base.GameController.StartCoroutine(routine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
                //base.GameController.ExhaustCoroutine(routine2);
            }
        }
    }
}