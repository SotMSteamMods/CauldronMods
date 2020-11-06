using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Cauldron.TheKnight
{
    public class KnightsHonorCardController : TheKnightCardController
    {
        public KnightsHonorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //TODO - Promo Support
            base.AddRedirectDamageTrigger(dd => IsThisCardNextToCard(dd.Target), c => base.CharacterCard, false);
            base.AddMakeDamageIrreducibleTrigger(dd => dd.Target == base.CharacterCard && dd.NumberOfTimesRedirected > 0 && IsThisCardNextToCard(dd.OriginalTarget));
            Card cardThisCardIsNextTo = base.GetCardThisCardIsNextTo(true);
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, cardThisCardIsNextTo != null && !cardThisCardIsNextTo.IsHeroCharacterCard);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a target.",
            //"Whenever that target would take damage, redirect that damage to {TheKnight}. Damage redirected this way is irreducible."
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget, "targets"), storedResults, isPutIntoPlay, decisionSources);
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

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy this card."
            IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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
