using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class AbductAndAbandonCardController : CardController
    {
        public AbductAndAbandonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.MoveCardOnDeck, GameController.GetAllCards(),
                additionalCriteria: c => c.IsInPlay && !c.IsCharacter && !GameController.IsCardIndestructible(c) && !c.IsOneShot && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                cardSource: GetCardSource());
            var coroutine = GameController.SelectCardAndDoAction(scd, SelectCardResponse);
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

        private IEnumerator SelectCardResponse(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                var card = scd.SelectedCard;

                var coroutine = GameController.MoveCard(DecisionMaker, card, card.Owner.Deck,
                                    showMessage: true,
                                    decisionSources: new IDecision[] { scd },
                                    evenIfIndestructible: false,
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
            yield break;
        }
    }
}