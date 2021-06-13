using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class VulcansJudgmentCardController : TitanCardController
    {
        public VulcansJudgmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When this card is destroyed, {Titan} deals 1 villain target 5 infernal damage. If Titanform is in play, {Titan} also deals that target 2 fire damage.
            base.AddWhenDestroyedTrigger(this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction action)
        {
            List<SelectCardDecision> targetDecision = new List<SelectCardDecision>();
            //{Titan} deals 1 villain target 5 infernal damage.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 5, DamageType.Infernal, 1, false, 1,
                                        additionalCriteria: c => IsVillainTarget(c),
                                        storedResultsDecisions: targetDecision,
                                        cardSource: GetNonPowerCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // If Titanform is in play...
            if (base.GetTitanform().Location.IsInPlayAndNotUnderCard && DidSelectCard(targetDecision))
            {
                var target = GetSelectedCard(targetDecision);
                //...{Titan} also deals that target 2 fire damage.
                coroutine = DealDamage(CharacterCard, target, 2, DamageType.Fire, cardSource: GetNonPowerCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Destroy this card.
            IEnumerator coroutine = GameController.DestroyCard(HeroTurnTakerController, Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private CardSource GetNonPowerCardSource(StatusEffect statusEffectSource = null)
        {
            bool? isFlipped = CardWithoutReplacements.IsFlipped;
            if (AllowActionsFromOtherSide)
            {
                isFlipped = null;
            }
            Power powerSource = null;
            List<string> villainCharacterIdentifiers = new List<string>();
            CardSource cardSource = new CardSource(this, isFlipped, canPerformActionsFromOtherSide: false, AssociatedCardSources, powerSource, CardSourceLimitation, AssociatedTriggers, null, villainCharacterIdentifiers, ActionSources, statusEffectSource);
            CardSource cardSource2 = GameController.DoesCardSourceGetReplaced(cardSource);
            if (cardSource2 != null)
            {
                return cardSource2;
            }
            return cardSource;
        }
    }
}