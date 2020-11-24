using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Gray
{
    public class GrayCharacterCardController : VillainCharacterCardController
    {

        public GrayCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), "radiation"));
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }
        public override void AddSideTriggers()
        {
            //Front - Walking Nuclear Reactor
            if (!base.Card.IsFlipped)
            {
                //At the end of the villain turn, if there are 3 or more radiation cards in play, flip {Gray}'s villain character cards and destroy 1 environment card.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, FlipCardResponse, TriggerType.FlipCard, additionalCriteria: (PhaseChangeAction action) => this.FindNumberOfRadiationCardsInPlay() >= 3));
                //At the end of the villain turn, {Gray} deals the hero target with the highest HP {H - 1} energy damage.
                base.AddSideTrigger(base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, Game.H - 1, DamageType.Energy));
                //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
                //Advanced - Whenever a radiation card is destroyed, destroy a second hero ongoing or equipment card.
                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.WasCardDestroyed && action.CardToDestroy.Card.DoKeywordsContain("radiation"), this.DestroyRadiationFrontResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage }, TriggerTiming.After));
            }
            //Back - Catastrophic Meltdown
            else
            {
                //At the start of the villain turn, if there are 1 or 0 radiation cards in play, flip {Gray}'s character cards.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, FlipCardResponse, TriggerType.FlipCard, additionalCriteria: (PhaseChangeAction action) => this.FindNumberOfRadiationCardsInPlay() <= 1));
                //At the start of the villain turn, destroy all but 2 hero ongoing or equipment cards. {Gray} deals each hero target {H x 2} energy damage. Play the top card of the villain deck.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.FlippedStartOfTurnResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage, TriggerType.PlayCard }));
                //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
                //Whenever a copy of Radioactive Cascade is destroyed, {Gray} deals the hero with the highest HP {H - 1} energy damage.
                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.WasCardDestroyed && action.CardToDestroy.Card.DoKeywordsContain("radiation"), this.DestroyRadiationBackResponse, TriggerType.DealDamage, TriggerTiming.After));
                //Advanced - Reduce damage dealt to villain targets by 1.
                if (Game.IsAdvanced)
                {
                    base.AddSideTrigger(base.AddReduceDamageTrigger((Card c) => c.IsVillain, 1));
                }
            }
            AddDefeatedIfDestroyedTriggers();
        }

        private int? FindNumberOfRadiationCardsInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), false, null, false).Count<Card>());
        }

        private IEnumerator FlipCardResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If flipping from front to back then destroy 1 environment after flipping
            if (base.Card.IsFlipped && FindNumberOfEnvironmentInPlay() > 0)
            {
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), false);
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

        private IEnumerator DestroyRadiationFrontResponse(DestroyCardAction action)
        {
            IEnumerator coroutine;
            //...destroy 1 hero ongoing or equipment card and {Gray} deals each non-villain target {H - 1} energy damage
            if (FindNumberOfHeroOngoingAndEquipmentInPlay() > 0)
            {
                int? numberOfCards = new int?(1);
                if(Game.IsAdvanced)
                {
                    //Advanced - Whenever a radiation card is destroyed, destroy a second hero ongoing or equipment card.
                    numberOfCards = new int?(2);
                }
                coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c))), numberOfCards, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            coroutine = base.DealDamage(base.Card, (Card c) => c.IsNonVillainTarget, Game.H - 1, DamageType.Energy);
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

        private int? FindNumberOfHeroOngoingAndEquipmentInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsHero && (c.IsOngoing || base.IsEquipment(c)), false, null, false).Count<Card>());
        }

        private int? FindNumberOfEnvironmentInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsEnvironment && c.IsInPlay, false, null, false).Count<Card>());
        }

        private IEnumerator FlippedStartOfTurnResponse(PhaseChangeAction action)
        {
            //...destroy all but 2 hero ongoing or equipment cards. 
            IEnumerator coroutine;
            while (this.FindNumberOfHeroOngoingAndEquipmentInPlay() > 2)
            {
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c))), false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //...{Gray} deals each hero target {H x 2} energy damage.
            coroutine = base.GameController.DealDamage(this.DecisionMaker, base.Card, (Card c) => c.IsHero, Game.H * 2, DamageType.Energy, cardSource: base.GetCardSource());
            //...Play the top card of the villain deck.
            IEnumerator coroutine2 = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        private IEnumerator DestroyRadiationBackResponse(DestroyCardAction action)
        {
            TurnTaker responsibleTurnTaker;
            if (action.ResponsibleCard != null)
            {
                responsibleTurnTaker = action.ResponsibleCard.Owner;
            }
            else
            {
                responsibleTurnTaker = base.TurnTaker;
            }
            //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
            IEnumerator coroutine;
            //if its not a hero destroying it do no damage
            if (responsibleTurnTaker != null && responsibleTurnTaker.IsHero)
            {
                if (responsibleTurnTaker.CharacterCard.IsInPlayAndHasGameText && !responsibleTurnTaker.CharacterCard.IsIncapacitatedOrOutOfGame)
                {
                    coroutine = base.DealDamage(base.Card, action.ResponsibleCard.Owner.CharacterCard, Game.H - 1, DamageType.Energy);
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
            //Whenever a copy of Radioactive Cascade is destroyed, {Gray} deals the hero with the highest HP {H - 1} energy damage.
            if (action.CardToDestroy.Card.Identifier == "RadioactiveCascade")
            {
                coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(Game.H - 1), DamageType.Energy);
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