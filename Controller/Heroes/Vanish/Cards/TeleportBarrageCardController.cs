using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class TeleportBarrageCardController : CardController
    {
        public TeleportBarrageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targets1 = GetPowerNumeral(0, 1);
            int damages1 = GetPowerNumeral(1, 2);
            int discards = GetPowerNumeral(2, 3);
            
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), damages1, DamageType.Energy, targets1, false, targets1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var selectCardsDecision = new SelectCardsDecision(GameController, DecisionMaker, c => c.IsInLocation(DecisionMaker.HeroTurnTaker.Hand), SelectionType.DiscardCard, discards, false, 0,
                eliminateOptions: true,
                cardSource: GetCardSource());
            coroutine = GameController.SelectCardsAndDoAction(selectCardsDecision, (SelectCardDecision d) => DiscardAndDealDamage(d), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DiscardAndDealDamage(SelectCardDecision scd)
        {
            int targets2 = GetPowerNumeral(3, 1);
            int damages2 = GetPowerNumeral(4, 1);

            if (scd.SelectedCard != null)
            {
                List<DiscardCardAction> result = new List<DiscardCardAction>();
                var coroutine = GameController.DiscardCard(scd.HeroTurnTakerController, scd.SelectedCard, new IDecision[] { scd }, storedResults: result, cardSource: scd.CardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidDiscardCards(result, 1))
                {
                    var damageSource = new DamageSource(GameController, CharacterCard);
                    coroutine = GameController.SelectTargetsAndDealDamage(scd.HeroTurnTakerController, damageSource, damages2, DamageType.Energy, targets2, false, targets2, cardSource: scd.CardSource);
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
    }
}