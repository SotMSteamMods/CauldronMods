using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Cauldron.Tiamat
{
    public class HydraNoxiousFireTiamatInstructionsCardController : HydraTiamatInstructionsCardController
    {
        public HydraNoxiousFireTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "HydraInfernoTiamatCharacter", "HydraDecayTiamatCharacter", "ElementOfFire")
        {

            SpecialStringMaker.ShowSpecialString(() => "The heroes may not win the game.").Condition = () => !base.Card.IsFlipped;
            SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => IsHead(c) && c.IsFlipped, "", useCardsSuffix: false, singular: "decapitated head", plural: "decapitated heads")).Condition = () => base.Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2).Condition = () => base.Card.IsFlipped && !FirstHeadCardController().Card.IsFlipped;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "AcidBreath", "acid breath")).Condition = () => base.Card.IsFlipped && FirstHeadCardController().Card.IsFlipped && !SecondHeadCardController().Card.IsFlipped && SecondHeadCardController().Card.IsInPlayAndNotUnderCard;
        }

        //Whenever Element of Fire enters play and {InfernoTiamatCharacter} is decapitated, if {DecayTiamatCharacter} is active she deals each hero target X toxic damage, where X = 2 plus the number of Acid Breaths in the villain trash.
        protected override IEnumerator alternateElementCoroutine => base.DealDamage(base.SecondHeadCardController().Card, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => this.PlusNumberOfACardInTrash(2, "AcidBreath"), DamageType.Toxic);
        private List<HydraTiamatInstructionsCardController> AllInstructionCardControllers
        {
            get
            {
                var controllers = new List<HydraTiamatInstructionsCardController>();
                var instructionCards = TurnTaker.GetAllCards(false).Where((Card c) => c.IsInPlayAndHasGameText && c.IsCharacter);
                foreach(Card card in instructionCards)
                {
                    if(GameController.FindCardController(card) is HydraTiamatInstructionsCardController instructionController)
                    {
                        controllers.Add(instructionController);
                    }
                }
                return controllers;
            }
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            {
                //The heroes may not win the game.
                base.AddTrigger<GameOverAction>((GameOverAction action) => action.ResultIsVictory, (GameOverAction action) => base.CancelAction(action), TriggerType.GameOver, TriggerTiming.Before),
                //At the end of the villain turn, {InfernoTiamatCharacter} deals the hero target with the highest HP 2 fire damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !base.FirstHeadCardController().Card.IsFlipped)
            };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
                //The first time {InfernoTiamatCharacter} deals damage each turn, increase that damage by 2.
                base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.FirstHeadCardController().Card && !this.DidDealDamageThisTurn(base.FirstHeadCardController().Card), 2)
            };
        }

        private bool IsPotentialGameLoser(GameAction action)
        {
            if(action is FlipCardAction fc)
            {
                return IsHead(fc.CardToFlip.Card) && fc.CardToFlip.Card.IsFlipped;
            }
            if(action is MoveCardAction mc)
            {
                return IsHead(mc.CardToMove) && mc.Destination.IsOutOfGame;
            }
            return false;
        }
        protected override ITrigger[] AddBackTriggers()
        {
            return new ITrigger[]
            {

                //The heroes win the game when 6 heads are decapitated.
                base.AddTrigger<GameAction>(delegate (GameAction action)
                {
                    if(IsPotentialGameLoser(action))
                    {
                         if(TurnTaker.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && IsHead(c)).Count() < 6)
                         {
                             return false;
                         }
                         return base.FindCardsWhere((Card c) => IsHead(c) && c.IsFlipped).Count() == 6;
                    }
                    return false;
                }, (GameAction action) => this.VictoryResponse(action), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After),
                //At the end of the villain turn, if {InfernoTiamatCharacter} is active, she deals the hero target with the second highest HP 1 fire damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !base.FirstHeadCardController().Card.IsFlipped)
            };
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = null;
            if (!base.Card.IsFlipped)
            {//Front End of Turn Damage
                //...{InfernoTiamatCharacter} deals the hero target with the highest HP 2 fire damage.
                coroutine = base.DealDamageToHighestHP(base.FirstHeadCardController().Card, 1, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => new int?(2), DamageType.Fire);
            }
            else
            {//Back End of Turn Damage
                //At the end of the villain turn, if {InfernoTiamatCharacter} is active, she deals the hero target with the second highest HP 1 fire damage.
                coroutine = base.DealDamageToHighestHP(base.FirstHeadCardController().Card, 2, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => new int?(1), DamageType.Fire);
            }
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
            IEnumerator coroutine = base.GameController.GameOver(EndingResult.VillainDestroyedVictory, "The heroes win!", cardSource: base.GetCardSource());
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