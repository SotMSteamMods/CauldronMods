using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheOldKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public TheOldKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheOldKnightCharacter} deals 1 target 2 irreducible lightning damage. Return 1 of your equipment cards in play to your hand."
            int targets = GetPowerNumeral(0, 1);
            int damage = GetPowerNumeral(1, 2);
            int returns = GetPowerNumeral(2, 1);
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), damage, DamageType.Lightning, targets, false, targets, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var selectCards = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => c.IsInPlayAndHasGameText && IsEquipment(c) && c.Owner == this.TurnTaker, SelectionType.ReturnToHand, returns, false, returns, cardSource: GetCardSource());
            coroutine = GameController.SelectCardsAndDoAction(selectCards, ReturnEquipmentToHand, cardSource: GetCardSource());
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

        private IEnumerator ReturnEquipmentToHand(SelectCardDecision scd)
        {
            IEnumerator coroutine = GameController.MoveCard(DecisionMaker, scd.SelectedCard, HeroTurnTaker.Hand, cardSource: GetCardSource());
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
        //"flippedBody": "When {TheOldKnightCharacter} flips to this side, destroy all equipment cards next to him.",
    }
}