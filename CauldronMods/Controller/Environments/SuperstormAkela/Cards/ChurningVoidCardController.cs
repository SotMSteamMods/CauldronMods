using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class ChurningVoidCardController : SuperstormAkelaCardController
    {

        public ChurningVoidCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            base.SpecialStringMaker.ShowSpecialString(() => BuildCardsLeftOfThisSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public readonly string HasTriggeredBeforeKey = "HasTriggeredBefore";

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals the { H} targets with the highest HP X projectile damage each, where X is the number of environment cards to the left of this one.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);

            //After all other start of turn effects have taken place, move this card 1 space to the right in the environment play area.
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction p) => p.FromPhase.TurnTaker == base.TurnTaker && p.FromPhase.IsStart && !HasBeenSetToTrueThisTurn(HasTriggeredBeforeKey), MoveCardResponse, TriggerType.MoveCard, TriggerTiming.Before);

        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //this card deals the { H} targets with the highest HP X projectile damage each, where X is the number of environment cards to the left of this one.
            Func<Card, int?> X = (Card c) => GetNumberOfCardsToTheLeftOfThisOne(base.Card) ?? 0;
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsTarget, X, DamageType.Projectile, numberOfTargets: () => Game.H);
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

        private IEnumerator MoveCardResponse(PhaseChangeAction pca)
        {
            //move this card 1 space to the right in the environment play area.
            IEnumerator coroutine = MoveCardOneToTheRight(base.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            SetCardPropertyToTrueIfRealAction(HasTriggeredBeforeKey);
            yield break;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible
            return card == base.Card;
        }


    }
}