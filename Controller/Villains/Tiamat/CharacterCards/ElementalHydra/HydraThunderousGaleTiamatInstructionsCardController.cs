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
        public HydraThunderousGaleTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.firstHead = base.GameController.FindCardController("HydraStormTiamatCharacter");
            this.secondHead = base.GameController.FindCardController("HydraWindTiamatCharacter");
            this.element = "ElementOfLightning";
            //Whenever Element of Lightning enters play and {StormTiamatCharacter} is decapitated, if {WindTiamatCharacter} is active she deals the X hero targets with the Highest HP {H - 1} projectile damage each, where X = 1 plus the number of ongoing cards in the villain trash.
            this.alternateElementCoroutine = base.DealDamageToHighestHP(this.secondHead.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(Game.H - 1), DamageType.Projectile, numberOfTargets: () => NumberOfOngoingsInTrash());
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            {
                //At the start of the villain turn, flip 1 decapitated head to its active side and restore it to 2 times {H} HP (3 times H if any Instruction is flipped and advanced).
                base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.GrowHeadResponse, TriggerType.FlipCard),
                //At the end of the villain turn, {StormTiamatCharacter} deals each hero target 1 lightning damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !firstHead.Card.IsFlipped)
            };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
                //The first time {StormTiamatCharacter} deals damage each turn, increase that damage by 2.
                base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == firstHead.Card && !this.DidDealDamageThisTurn(firstHead.Card), (DealDamageAction action) => 2)
            };
        }

        protected override ITrigger[] AddBackTriggers()
        {
            return new ITrigger[]
            {
                //At the start of the villain turn, flip 1 decapitated head to its active side and restore it to 2 times {H} HP (3 times H if any Instruction is flipped and advanced).
                base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.GrowHeadResponse, TriggerType.FlipCard),
                //At the end of the villain turn, {StormTiamatCharacter} deals each hero target 1 lightning damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !firstHead.Card.IsFlipped)
            };
        }

        private IEnumerator GrowHeadResponse(PhaseChangeAction action)
        {
            IEnumerable<Card> decapitatedHeads = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsFlipped && c.DoKeywordsContain("head") && c.IsInPlayAndNotUnderCard));
            SelectCardDecision cardDecision = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.CharacterCard, decapitatedHeads, cardSource: base.GetCardSource());
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
            if (Game.IsAdvanced && base.FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsVillainCharacterCard && c.IsInPlayAndNotUnderCard)).Count<Card>() > 3)
            {
                //Every Instruction card has this as its Advanced Flipped Game Text, if there are ever more than 3 heads in play then one of the instructions are flipped
                //Decapitated heads are restored to 3 times {H} HP when they become active.
                HP = Game.H * 3;
            }
            //...flip 1 decapitated head to its active side...
            IEnumerator coroutine = base.GameController.FlipCard(base.FindCardController(decision.SelectedCard), cardSource: base.GetCardSource()); ;
            //...and restore it to 2 times {H} HP.
            IEnumerator coroutine2 = base.GameController.MakeTargettable(secondHead.Card, decision.SelectedCard.MaximumHitPoints ?? default, HP, base.GetCardSource());
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
            //...{StormTiamatCharacter} deals each hero target 1 lightning damage.
            IEnumerator coroutine = base.DealDamage(this.firstHead.Card, (Card c) => c.IsHero, 1, DamageType.Lightning);
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
                    where card.IsOngoing
                    select card).Count<Card>();
        }
    }
}

