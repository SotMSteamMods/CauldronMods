using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class BerserkerTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public BerserkerTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy an equipment target. {TheKnight} deals 1 target X psychic damage, where X is the HP of the equipment before it was destroyed plus 1."
            int numTargets = GetPowerNumeral(0, 1);
            int numDamModifier = GetPowerNumeral(1, 1);
            var storedArmor = new List<SelectCardDecision> { };
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DestroyCard, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsEquipment(c) && c.IsTarget, "", false, false, singular:"equipment target", plural:"equipment targets"), storedArmor, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(storedArmor))
            {
                Card armor = storedArmor.FirstOrDefault().SelectedCard;
                int armorHP = armor.HitPoints ?? 0;
                coroutine = GameController.DestroyCard(DecisionMaker, armor, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), armorHP + numDamModifier, DamageType.Psychic, numTargets, false, numTargets, cardSource: GetCardSource());
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        //"Increase all damage by 1 until the start of your next turn.",
                        IncreaseDamageStatusEffect globalBoost = new IncreaseDamageStatusEffect(1);
                        globalBoost.UntilStartOfNextTurn(this.TurnTaker);
                        coroutine = GameController.AddStatusEffect(globalBoost, true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"One target deals itself 1 psychic damage."
                        coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 1, DamageType.Psychic, 1, false, 1, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}