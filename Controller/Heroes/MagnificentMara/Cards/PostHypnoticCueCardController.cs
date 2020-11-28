using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class PostHypnoticCueCardController : CardController
    {
        public PostHypnoticCueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a hero."
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && c.IsTarget, "hero character", useCardsSuffix: false), storedResults, isPutIntoPlay, decisionSources);
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
            //"When this card is destroyed, [the hero this is next to] hero may use a power.",
            AddWhenDestroyedTrigger(HeroMayUsePower, TriggerType.UsePower);
            //"At the start of your turn you may destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, YouMayDestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator HeroMayUsePower(DestroyCardAction dc)
        {
            Card hero = GetCardThisCardIsNextTo();
            if (hero != null)
            {
                var controller = FindCardController(hero);
                IEnumerator coroutine = SelectAndUsePower(controller, optional: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}