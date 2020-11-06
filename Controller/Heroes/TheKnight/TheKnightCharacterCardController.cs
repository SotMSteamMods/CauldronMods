using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheKnightCharacterCardController : HeroCharacterCardController
    {
        public TheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheKnight} deals 1 target 1 melee damage."
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), 1, DamageType.Melee, 1, false, 1, cardSource: base.GetCardSource());
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now."
                        coroutine = base.SelectHeroToPlayCard(this.HeroTurnTakerController);
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
                case 1:
                    {
                        //"Reduce damage dealt to 1 target by 1 until the start of your next turn."
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget, "targets in play", false), storedResults, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card selectedCard = GetSelectedCard(storedResults);
                            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = selectedCard;
                            reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;

                    }
                case 2:
                    {
                        //"Increase damage dealt by 1 target by 1 until the start of your next turn."
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget, "targets in play", false), storedResults, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card selectedCard = GetSelectedCard(storedResults);
                            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = selectedCard;
                            increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            coroutine = base.AddStatusEffect(increaseDamageStatusEffect, true);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}