using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class SaveTheDayCardController : WindmillCityUtilityCardController
    {
        public SaveTheDayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, play the top card of the environment deck.
            IEnumerator coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
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

        public override void AddTriggers()
        {
            //Whenever a hero card destroys a villain target, 1 Responder regains 1HP.
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && IsVillainTarget(dca.CardToDestroy.Card) && dca.GetCardDestroyer() != null && IsHero(dca.GetCardDestroyer()), VillainTargetDestroyedResponse, TriggerType.GainHP, TriggerTiming.After);
        }

        private IEnumerator VillainTargetDestroyedResponse(DestroyCardAction dca)
        {
            // 1 Responder regains 1HP.

            IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria(c => IsResponder(c) && c.IsInPlayAndHasGameText, "responder"), visibleToCard: GetCardSource());
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, DecisionMaker, SelectionType.GainHP, choices, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision decision) => GameController.GainHP(decision.SelectedCard, 1, cardSource: GetCardSource()));
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
