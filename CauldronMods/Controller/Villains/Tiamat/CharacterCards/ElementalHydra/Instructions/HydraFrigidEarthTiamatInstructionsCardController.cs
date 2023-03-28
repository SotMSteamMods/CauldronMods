using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Tiamat
{
    public class HydraFrigidEarthTiamatInstructionsCardController : HydraTiamatInstructionsCardController
    {
        public HydraFrigidEarthTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "HydraWinterTiamatCharacter", "HydraEarthTiamatCharacter", "ElementOfIce")
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsFlipped && FirstHeadCardController().Card.IsFlipped && !SecondHeadCardController().Card.IsFlipped && SecondHeadCardController().Card.IsInPlayAndNotUnderCard;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "SkyBreaker", "sky breaker")).Condition = () => base.Card.IsFlipped && FirstHeadCardController().Card.IsFlipped && !SecondHeadCardController().Card.IsFlipped && SecondHeadCardController().Card.IsInPlayAndNotUnderCard;
            SpecialStringMaker.ShowHeroTargetWithLowestHP().Condition = () => base.Card.IsFlipped && !FirstHeadCardController().Card.IsFlipped;
        }

        //Whenever Element of Ice enters play and {WinterTiamatCharacter} is decapitated, if {EarthTiamatCharacter} is active she deals the hero target with the highest HP X melee damage, where X = {H} plus the number of Sky Breaker cards in the villain trash.
        protected override IEnumerator alternateElementCoroutine => base.DealDamageToHighestHP(base.SecondHeadCardController().Card, 1, (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, (Card c) => this.PlusNumberOfACardInTrash(Game.H, "SkyBreaker"), DamageType.Melee);

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            {
                //At the end of the villain turn, 1 player must discard 1 card.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.DiscardResponse, TriggerType.DiscardCard)
            };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
                //At the end of the villain turn, 1 player must discard 1 card.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.DiscardResponse, TriggerType.DiscardCard)
            };
        }

        protected override ITrigger[] AddBackTriggers()
        {
            return new ITrigger[]
            {
                //At the end of the villain turn, if {WinterTiamatCharacter} is active, she deals the hero target with the lowest HP 1 cold damage.
                base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => !base.FirstHeadCardController().Card.IsFlipped)
            };
        }

        private IEnumerator DiscardResponse(PhaseChangeAction action)
        {
            //At the end of the villain turn, 1 player must discard 1 card.
            IEnumerator coroutine = base.GameController.SelectHeroToDiscardCard(this.DecisionMaker, optionalDiscardCard: false, cardSource: base.GetCardSource());
            //base.SelectAndDiscardCards(this.DecisionMaker, 1);
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

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...if {WinterTiamatCharacter} is active, she deals the hero target with the lowest HP 1 cold damage.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.FirstHeadCardController().Card, 1, (Card c) => IsHero(c), (Card c) => new int?(1), DamageType.Cold);
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