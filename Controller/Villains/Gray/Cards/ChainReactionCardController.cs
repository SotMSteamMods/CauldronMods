using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron
{
    public class ChainReactionCardController : CardController
    {
        public ChainReactionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the villain turn, this card deals the X hero targets with the lowest HP 1 energy damage each, where X is the number of Radiation cards in play.
            Func<Card, int?> X = (Card card) => new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), false, null, false).Count<Card>());
            base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, TargetType.LowestHP, 0, DamageType.Energy, dynamicAmount: X);
            //At the end of the villain turn, put a random Radiation card from the villain trash into play.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.BringRadiationBackResponse), TriggerType.PutIntoPlay);


        }
        private IEnumerator BringRadiationBackResponse(PhaseChangeAction phaseChange)
        {
            bool tryPlaying = true;
            string message = base.Card.Title + " plays a radiation card from the Trash.";
            if (!base.TurnTaker.Trash.Cards.Any((Card c) => c.IsCitizen))
            {
                tryPlaying = false;
                message = "There are no radiation cards in Gray's trash for him to play.";
            }
            IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource(null), null, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (tryPlaying)
            {
                IEnumerator coroutine2 = base.ReviveCardFromTrash(base.TurnTakerController, (Card c) => c.DoKeywordsContain("radiation"));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            yield break;
        }
    }
}