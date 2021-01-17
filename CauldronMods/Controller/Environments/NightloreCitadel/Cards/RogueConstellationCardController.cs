using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class RogueConstellationCardController : NightloreCitadelUtilityCardController
    {
        public RogueConstellationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP().Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, move it next to the villain target with the highest HP. 
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredHighestHitPoints(c, (Card card) => IsVillainTarget(card) && GameController.IsCardVisibleToCardSource(card, GetCardSource())) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero with the lowest hp"), storedResults, isPutIntoPlay, decisionSources);

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

        public override IEnumerator Play()
        {
            //Then, play the top card of the environment deck.
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
            //If the target next to this card leaves play, destroy this card.
            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }
    }
}
