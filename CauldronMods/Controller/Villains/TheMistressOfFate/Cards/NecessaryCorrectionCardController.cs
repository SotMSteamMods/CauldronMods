using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class NecessaryCorrectionCardController : TheMistressOfFateUtilityCardController
    {
        public NecessaryCorrectionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: Game.H - 1).Condition = () => !Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck);
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals the {H - 1} hero targets with the highest HP 10 psychic damage each.",
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), c => 10, DamageType.Psychic, numberOfTargets: () => H - 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"At the end of the environment turn, if there are at least 5 cards remaining in the villain deck, destroy this card and flip {TheMistressOfFate}'s villain character cards."
            AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment && TurnTaker.Deck.NumberOfCards >= 5, FlipMistressResponse, TriggerType.FlipCard);
        }

        private IEnumerator FlipMistressResponse(PhaseChangeAction pc)
        {
            IEnumerator message = GameController.SendMessageAction($"{Card.Title} activates!", Priority.Medium, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(message);
            }
            else
            {
                GameController.ExhaustCoroutine(message);
            }
            Func<DestroyCardAction, IEnumerator> afterDestroyFlip = (DestroyCardAction dc) => GameController.FlipCard(CharacterCardController, cardSource: GetCardSource(), allowBackToFront: false);
            AddWhenDestroyedTrigger(afterDestroyFlip, new TriggerType[] { TriggerType.FlipCard });
            IEnumerator coroutine = DestroyThisCardResponse(pc);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            RemoveWhenDestroyedTriggers();

            //in case it was indestructible or something and is still in play
            coroutine = GameController.FlipCard(CharacterCardController, cardSource: GetCardSource(), allowBackToFront: false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
