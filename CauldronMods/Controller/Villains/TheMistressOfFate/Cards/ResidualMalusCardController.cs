using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ResidualMalusCardController : TheMistressOfFateUtilityCardController
    {
        private readonly string CardPlayedKey = "ResidualMalusCardPlayedThisTurnKey";
        public ResidualMalusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(() => environmentTTC.TurnTaker.Trash).Condition = () => Game.HasGameStarted;
            SpecialStringMaker.ShowHasBeenUsedThisTurn(CardPlayedKey);
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("WarpedMalus").Condition = () => Card.IsInPlayAndHasGameText;
        }
        private TurnTakerController environmentTTC => FindEnvironment();
        public override void AddTriggers()
        {
            //"The first time a hero card is played each turn, this card deals that hero X melee damage, where X is the number of cards in the environment trash plus 1.",
            AddTrigger((PlayCardAction pc) => pc.CardToPlay.IsHero && !pc.IsPutIntoPlay && !HasBeenSetToTrueThisTurn(CardPlayedKey), PlayCardDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            //"When this card is destroyed, destroy 1 Warped Malus."
            AddWhenDestroyedTrigger(DestroyWarpedMalusResponse, TriggerType.DestroyCard);
        }

        private IEnumerator PlayCardDamageResponse(PlayCardAction pc)
        {
            if(!HasBeenSetToTrueThisTurn(CardPlayedKey))
            {
                SetCardPropertyToTrueIfRealAction(CardPlayedKey);
                var hero = pc.CardToPlay.Owner;
                if(hero != null)
                {
                    IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(FindHeroTurnTakerController(hero.ToHero()),
                                                                new DamageSource(GameController, this.Card),
                                                                (Card c) => FindEnvironment().TurnTaker.Trash.NumberOfCards + 1,
                                                                DamageType.Melee,
                                                                () => 1, false, 1,
                                                                additionalCriteria: (Card c) => c.IsHeroCharacterCard && c.Owner == hero,
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
            yield break;
        }

        private IEnumerator DestroyWarpedMalusResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "WarpedMalus", "Warped Malus"), false, cardSource: GetCardSource());
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
