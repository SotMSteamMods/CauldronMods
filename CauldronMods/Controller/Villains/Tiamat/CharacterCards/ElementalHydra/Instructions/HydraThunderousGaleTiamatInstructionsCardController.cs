using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Tiamat
{
    public class HydraThunderousGaleTiamatInstructionsCardController : HydraTiamatInstructionsCardController
    {
        public HydraThunderousGaleTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "HydraStormTiamatCharacter", "HydraWindTiamatCharacter", "ElementOfLightning")
        {
            SpecialStringMaker.ShowSpecialString(() => BuildDecapitatedHeadList());
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: 1 + NumberOfOngoingsInTrash()).Condition = () => base.Card.IsFlipped && FirstHeadCardController().Card.IsFlipped && !SecondHeadCardController().Card.IsFlipped && SecondHeadCardController().Card.IsInPlayAndNotUnderCard;
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsFlipped && !FirstHeadCardController().Card.IsFlipped;

            //putting the challenge rule logic here for no particular reason
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        //Whenever Element of Lightning enters play and {StormTiamatCharacter} is decapitated, if {WindTiamatCharacter} is active she deals the X hero targets with the Highest HP {H - 1} projectile damage each, where X = 1 plus the number of ongoing cards in the villain trash.
        protected override IEnumerator alternateElementCoroutine => base.DealDamageToHighestHP(base.SecondHeadCardController().Card, 1, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => new int?(Game.H - 1), DamageType.Projectile, numberOfTargets: () => 1 + NumberOfOngoingsInTrash());

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            {
                //At the start of the villain turn, flip 1 decapitated head to its active side and restore it to 2 times {H} HP (3 times H if any Instruction is flipped and advanced).
                base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.GrowHeadResponse, TriggerType.FlipCard),
                //At the end of the villain turn, {StormTiamatCharacter} deals each hero target 1 lightning damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !base.FirstHeadCardController().Card.IsFlipped),
                //Cleans up indestructible cards that need it, once they stop being indestructible
                ChallengeLoseIndestructibleTrigger()
            };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
                //The first time {StormTiamatCharacter} deals damage each turn, increase that damage by 2.
                base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.FirstHeadCardController().Card && !this.DidDealDamageThisTurn(base.FirstHeadCardController().Card), (DealDamageAction action) => 2)
            };
        }

        protected override ITrigger[] AddBackTriggers()
        {
            return new ITrigger[]
            {
                //At the start of the villain turn, flip 1 decapitated head to its active side and restore it to 2 times {H} HP (3 times H if any Instruction is flipped and advanced).
                base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.GrowHeadResponse, TriggerType.FlipCard),
                //At the end of the villain turn, if {StormTiamatCharacter} is active, it the hero target with the highest HP 1 lightning damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !base.FirstHeadCardController().Card.IsFlipped),
                //Cleans up indestructible cards that need it, once they stop being indestructible
                ChallengeLoseIndestructibleTrigger()
            };
        }

        private IEnumerator GrowHeadResponse(PhaseChangeAction action)
        {
            IEnumerable<Card> decapitatedHeads = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsFlipped && IsHead(c) && c.IsInPlayAndNotUnderCard));
            SelectCardDecision cardDecision = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.FlipCardFaceUp, decapitatedHeads, cardSource: base.GetCardSource());
            //...flip 1 decapitated head...
            IEnumerator coroutine = base.GameController.SelectCardAndDoAction(cardDecision, (SelectCardDecision decision) => this.GrowHeadAction(decision));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            };
            yield break;
        }

        private IEnumerator GrowHeadAction(SelectCardDecision decision)
        {
            int HP = Game.H * 2;
            if (Game.IsAdvanced && base.FindCardsWhere(new LinqCardCriteria((Card c) => IsHead(c) && c.IsVillainCharacterCard && c.IsInPlayAndNotUnderCard)).Count() > 3)
            {
                //Every Instruction card has this as its Advanced Flipped Game Text, if there are ever more than 3 heads in play then one of the instructions are flipped
                //Decapitated heads are restored to 3 times {H} HP when they become active.
                HP = Game.H * 3;
            }
            //...flip 1 decapitated head to its active side...
            IEnumerator coroutine = base.GameController.FlipCard(base.FindCardController(decision.SelectedCard), cardSource: base.GetCardSource()); ;
            //...and restore it to 2 times {H} HP.
            IEnumerator coroutine2 = base.GameController.MakeTargettable(decision.SelectedCard, 15, HP, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            };
            yield break;
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = null;
            if (!base.Card.IsFlipped)
            {//Front End of Turn Damage
                //...{StormTiamatCharacter} deals each hero target 1 lightning damage.
                coroutine = base.DealDamage(base.FirstHeadCardController().Card, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, 1, DamageType.Lightning);
            }
            else
            {//Back End of Turn Damage
                //At the end of the villain turn, if {StormTiamatCharacter} is active, she deals the hero target with the highest HP 1 lightning damage.
                coroutine = base.DealDamageToHighestHP(base.FirstHeadCardController().Card, 1, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => new int?(1), DamageType.Lightning);
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

        protected int NumberOfOngoingsInTrash()
        {
            return (from card in base.TurnTaker.Trash.Cards
                    where IsOngoing(card)
                    select card).Count();
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //"Villain ongoings are indestructible as long as 2 or more heads are not decapitated.",
            if (Game.IsChallenge && card != null && card.IsVillain && IsOngoing(card))
            {
                return DoesChallengeIndestructibleApply;
            }
            return false;
        }
        private ITrigger ChallengeLoseIndestructibleTrigger()
        {
            return AddTrigger((FlipCardAction fc) => Game.IsChallenge && IsHead(fc.CardToFlip.Card) && fc.CardToFlip.Card.IsFlipped && !DoesChallengeIndestructibleApply, fc => GameController.DestroyAnyCardsThatShouldBeDestroyed(cardSource: fc.CardToFlip.GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After);
        }
        private bool DoesChallengeIndestructibleApply => TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsHead(c) && !c.IsFlipped).Count() >= 2;
    }
}

