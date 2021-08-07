using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller.OblivAeon;

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
                additionalCriteria: c => c.IsInPlay && !c.IsCharacter && !c.IsOneShot && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (FindCardController(c) is MissionCardController ? c.IsFlipped : true),
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

                Location destination = card.NativeDeck is null || card.NativeDeck.OwnerTurnTaker != card.Owner ? card.Owner.Deck : card.NativeDeck;
              
                var coroutine = GameController.MoveCard(DecisionMaker, card, destination,
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