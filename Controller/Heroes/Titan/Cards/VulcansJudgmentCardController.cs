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
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 5, DamageType.Infernal, 1, false, 1, storedResultsDecisions: targetDecision, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // If Titanform is in play...
            if (base.GetTitanform().Location.IsInPlayAndNotUnderCard)
            {
                //...{Titan} also deals that target 2 fire damage.
                coroutine = base.DealDamage(base.CharacterCard, targetDecision.FirstOrDefault().SelectedCard, 2, DamageType.Fire, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Destroy this card.
            IEnumerator coroutine = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}