using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    class MagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
    {
        public MagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetAmount = GetPowerNumeral(0, 1);
            int firstTargetDamage = GetPowerNumeral(1, 1);
            int secondTargetDamage = GetPowerNumeral(2, 1);
            //"{MagnificentMara} deals 1 target 1 psychic damage. That target deals another target 1 melee damage."
            List<SelectCardDecision> firstTargetDecision = new List<SelectCardDecision> { };
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), firstTargetDamage, DamageType.Psychic, targetAmount, false, targetAmount, selectTargetsEvenIfCannotDealDamage: true, storedResultsDecisions: firstTargetDecision, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(firstTargetDecision))
            {
                yield break;
            }

            Card firstTarget = firstTargetDecision.FirstOrDefault().SelectedCard; 

            if (firstTarget != null && firstTarget.IsInPlayAndHasGameText && firstTarget.IsTarget)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, firstTarget), secondTargetDamage, DamageType.Psychic, 1, false, 1, additionalCriteria: (Card c) => c != firstTarget, cardSource: GetCardSource());
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
            switch(index)
            {
                case (0):
                    {
                        //"One hero may put a card from their trash on top of their deck.",

                        break;
                    }
                case (1):
                    {
                        //"Destroy 1 ongoing card.",

                        break;
                    }
                case (2):
                    {
                        //"Select a target. Until the start of your next turn that target is immune to damage from environment cards."

                        break;
                    }
            }
            yield break;
        }
    }
}
