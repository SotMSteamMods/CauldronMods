using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class MisdirectionCardController : CardController
    {
        public MisdirectionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{MagnificentMara} deals 1 target 1 sonic damage.",
            var storedDamage = new List<DealDamageAction> { };
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), 1, DamageType.Sonic, 1, false, 1, selectTargetsEvenIfCannotDealDamage: true, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedDamage.FirstOrDefault() == null || storedDamage.FirstOrDefault().Target == null)
            {
                yield break;
            }
            //"One other hero target deals that same target 2 damage of a type of their choosing."
            var heroSource = new List<SelectCardDecision> { };
            var previewDamage = new DealDamageAction(GetCardSource(), null, null, 2, DamageType.Melee);
            //coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsHeroTarget(c) && c != this.CharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource())), heroSource, damageAmount: (Card c) => 2, damageType: DamageType.Melee, selectionType: SelectionType.CardToDealDamage, cardSource: GetCardSource());
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsHeroTarget(c) && c != this.CharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero target"), heroSource, optional: false, allowAutoDecide: false, previewDamage, includeRealCardsOnly: true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (heroSource.FirstOrDefault() != null)
            {
                var sourceCard = heroSource.FirstOrDefault().SelectedCard;
                var otherHero = FindHeroTurnTakerController(sourceCard.Owner.ToHero());
                var storedType = new List<SelectDamageTypeDecision> { };
                coroutine = GameController.SelectDamageType(otherHero, storedType, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                DamageType chosenType = GetSelectedDamageType(storedType) == null ? DamageType.Melee : (DamageType)GetSelectedDamageType(storedType);
                coroutine = GameController.DealDamage(otherHero, sourceCard, (Card c) => c == storedDamage.FirstOrDefault().Target, 2, chosenType, cardSource: GetCardSource());
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