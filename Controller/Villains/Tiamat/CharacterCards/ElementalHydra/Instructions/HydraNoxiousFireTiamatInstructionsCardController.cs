using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public class HydraNoxiousFireTiamatInstructionsCardController : HydraTiamatInstructionsCardController
    {
        public HydraNoxiousFireTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            this.firstHead = base.GameController.FindCardController("HydraInfernoTiamatCharacter");
            this.secondHead = base.GameController.FindCardController("HydraDecayTiamatCharacter");
            this.element = "ElementOfFire";
            //Whenever Element of Fire enters play and {InfernoTiamatCharacter} is decapitated, if {DecayTiamatCharacter} is active she deals each hero target X toxic damage, where X = 2 plus the number of Acid Breaths in the villain trash.
            this.alternateElementCoroutine = base.DealDamage(this.secondHead.Card, (Card c) => c.IsHero, this.PlusNumberOfACardInTrash(2, "AcidBreath"), DamageType.Toxic);
            yield break;
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            {
                //The heroes may not win the game.
                base.AddTrigger<GameOverAction>((GameOverAction action) => action.ResultIsVictory, (GameOverAction action) => base.CancelAction(action), TriggerType.GameOver, TriggerTiming.Before),
                //At the end of the villain turn, {InfernoTiamatCharacter} deals the hero target with the highest HP 2 fire damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !firstHead.Card.IsFlipped)
            };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
                //The first time {InfernoTiamatCharacter} deals damage each turn, increase that damage by 2.
                base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == firstHead.Card && !this.DidDealDamageThisTurn(firstHead.Card), (DealDamageAction action) => 2)
            };
        }

        protected override ITrigger[] AddBackTriggers()
        {
            return new ITrigger[]
            {
                //The heroes win the game when 6 heads are decapitated.
                base.AddTrigger<GameAction>(delegate (GameAction action)
                {
                    if (base.GameController.HasGameStarted && !(action is GameOverAction) && !(action is IncrementAchievementAction))
                    {
                        return base.FindCardsWhere((Card c) => c.DoKeywordsContain("head") && c.IsFlipped).Count<Card>() == 6;
                    }
                    return false;
                }, (GameAction action) => this.VictoryResponse(action), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After),
                //At the end of the villain turn, if {InfernoTiamatCharacter} is active, she deals the hero target with the second highest HP 1 fire damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !firstHead.Card.IsFlipped)
            };
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{InfernoTiamatCharacter} deals the hero target with the highest HP 2 fire damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(this.firstHead.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(2), DamageType.Fire);
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

        private IEnumerator VictoryResponse(GameAction action)
        {
            IEnumerator coroutine = base.GameController.GameOver(EndingResult.AlternateVictory, "The heroes win!", cardSource: base.GetCardSource());
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