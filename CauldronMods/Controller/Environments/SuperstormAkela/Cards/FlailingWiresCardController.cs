using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class FlailingWiresCardController : SuperstormAkelaCardController
    {

        public FlailingWiresCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => BuildCardsLeftOfThisSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;

        }

        public override IEnumerator Play()
        {
            //"When this card enters play, play the top card of the environment deck.",
            Log.Debug(Card.Title + " plays the top card of the environment deck...");
            IEnumerator coroutine = GameController.SendMessageAction(Card.Title + " plays the top card of the environment deck...", Priority.High, GetCardSource(),showCardSource: true);
            IEnumerator coroutine2 = GameController.PlayTopCard(DecisionMaker, base.TurnTakerController,cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
                yield return GameController.StartCoroutine(coroutine2);

            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
                GameController.ExhaustCoroutine(coroutine2);
            }
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP 1 lightning damage each, where X is the number of environment cards to the left of this one.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            // this card deals the X+1 hero targets with the highest HP 1 lightning damage each, where X is the number of environment cards to the left of this one.

            Func<int> numTargets = () => (GetNumberOfCardsToTheLeftOfThisOne(base.Card) ?? 0) + 1;
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => new int?(1), DamageType.Lightning, numberOfTargets: numTargets);
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