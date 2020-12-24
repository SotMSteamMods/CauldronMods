using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class KnightsHonorCardController : TheKnightCardController
    {
        public KnightsHonorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger((DealDamageAction dd) => dd.Amount > 0 && IsThisCardNextToCard(dd.Target), RedirectToKnight, TriggerType.RedirectDamage, TriggerTiming.Before);

            Card cardThisCardIsNextTo = base.GetCardThisCardIsNextTo(true);
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, cardThisCardIsNextTo != null && !cardThisCardIsNextTo.IsHeroCharacterCard);
        }

        private IEnumerator RedirectToKnight(DealDamageAction dd)
        {
            if(!dd.IsRedirectable)
            {
                yield break;
            }
            var selectedKnight = new List<SelectCardDecision> { };
            IEnumerator coroutine = SelectOwnCharacterCard(selectedKnight, SelectionType.RedirectDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(!DidSelectCard(selectedKnight))
            {
                yield break;
            }

            Card knight = selectedKnight.FirstOrDefault().SelectedCard;
            RedirectDamageAction redirect = new RedirectDamageAction(GetCardSource(), dd, knight);
            coroutine = DoAction(redirect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(dd.DamageModifiers.Any((ModifyDealDamageAction mdda) => mdda.CardSource != null && mdda.CardSource.Card == this.Card))
            {
                var irreducible = new MakeDamageIrreducibleAction(GetCardSource(), dd);
                coroutine = DoAction(irreducible);
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
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a target.",
            //"Whenever that target would take damage, redirect that damage to {TheKnight}. Damage redirected this way is irreducible."
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText, "targets", false), storedResults, isPutIntoPlay, decisionSources);
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
