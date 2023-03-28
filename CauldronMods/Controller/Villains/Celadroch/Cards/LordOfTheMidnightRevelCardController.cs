using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LordOfTheMidnightRevelCardController : CardController
    {
        /*
         *  "The first time another villain target would be dealt damage each turn, this card deals the source of that damage {H} melee damage.",
			"When this card is destroyed, destroy 1 hero ongoing or equipment card for each other non-character villain target in play."
         */

        private static readonly string FirstTimeDamageDealtKey = "FirstTimeDamageDealtKey";

        public LordOfTheMidnightRevelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDamageDealtKey, $"{Card.Title} has dealt counter attack damage this turn.", $"{Card.Title} has not dealt counter attack damage this turn.");
            SpecialStringMaker.ShowSpecialString(CardsThatWillbeDestroyedSpecialString);
        }

        private string CardsThatWillbeDestroyedSpecialString()
        {
            int count = GetAmountOfCardsToDestoy();
            switch (count)
            {
                case 1:
                    return $"When destroyed, {Card.Title} will destroy 1 hero ongoing or equipment card";
                default:
                    return $"When destroyed, {Card.Title} will destroy {count} hero ongoing or equipment cards";
            }
        }

        private int GetAmountOfCardsToDestoy()
        {
            return FindCardsWhere(c => IsVillainTarget(c) && !c.IsCharacter && c != Card && c.IsInPlay, visibleToCard: GetCardSource()).Count();
        }

        private bool IsCounterDamageAvailable()
        {
            return !Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == FirstTimeDamageDealtKey && j.BoolValue == true);
        }

        private bool CounterDamageCriteria(DealDamageAction dda)
        {
            return dda.Amount > 0 && dda.Target != Card && IsVillainTarget(dda.Target) && IsCounterDamageAvailable();
        }

        public override void AddTriggers()
        {
            // NB: Confirmed by Tosx that this is normal counter damage that occurs after the damage
            AddTrigger<DealDamageAction>(CounterDamageCriteria, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

            AddWhenDestroyedTrigger(dca => WhenDestroyedResponse(), TriggerType.DestroyCard);

            AddAfterLeavesPlayAction(() => ResetFlagAfterLeavesPlay(FirstTimeDamageDealtKey));
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealtKey);
            return DealDamage(Card, dda.DamageSource.Card, H, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource());
        }

        private IEnumerator WhenDestroyedResponse()
        {
            var coroutine = GameController.SelectAndDestroyCards(DecisionMaker,
                                new LinqCardCriteria(c => IsHero(c) && (IsOngoing(c) || IsEquipment(c)) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing or equipment"),
                                null,
                                dynamicNumberOfCards: GetAmountOfCardsToDestoy,
                                allowAutoDecide: true,
                                cardSource: GetCardSource());
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