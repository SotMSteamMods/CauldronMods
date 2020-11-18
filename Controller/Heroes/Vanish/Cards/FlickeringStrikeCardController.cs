using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class FlickeringStrikeCardController : CardController
    {
        public FlickeringStrikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var coroutine = GameController.DrawCards(this.DecisionMaker, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var selectCardsDecision = new SelectCardsDecision(GameController, DecisionMaker, c => c.IsInLocation(DecisionMaker.HeroTurnTaker.Hand), SelectionType.DiscardCard, null, false, 0, cardSource: GetCardSource());
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
            if (scd.SelectedCard != null)
            {
                List<DiscardCardAction> result = new List<DiscardCardAction>();
                var coroutine = GameController.DiscardCard(scd.HeroTurnTakerController, scd.SelectedCard, scd.ToEnumerable<IDecision>(), storedResults: result, cardSource: scd.CardSource);
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
                    coroutine = GameController.SelectTargetsAndDealDamage(scd.HeroTurnTakerController, damageSource, 1, DamageType.Energy, 1, false, 1, cardSource: scd.CardSource);
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