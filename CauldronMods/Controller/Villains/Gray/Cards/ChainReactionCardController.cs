using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Gray
{
    public class ChainReactionCardController : GrayCardController
    {
        public ChainReactionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation"), "radiation"));
            base.SpecialStringMaker.ShowLowestHP(1, () => base.FindNumberOfRadiationCardsInPlay().Value, new LinqCardCriteria((Card c) => IsHero(c), "hero", true, false, "target", "targets"));
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation"), "radiation"));
        }



        public override void AddTriggers()
        {
            //At the start of the villain turn, this card deals the X hero targets with the lowest HP 1 energy damage each, where X is the number of Radiation cards in play.
            //This does not affect a dynamic number of targets because the DealDamageToLowestHP does not accept a dynamic number of targets
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, (PhaseChangeAction action) => this.DealDamageToLowestHP(this.Card, 1, (Card c) => IsHero(c), (Card c) => new int?(1), DamageType.Energy, numberOfTargets: this.FindNumberOfRadiationCardsInPlay() ?? default), TriggerType.DealDamage);

            //At the end of the villain turn, put a random Radiation card from the villain trash into play.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.BringRadiationBackResponse), TriggerType.PutIntoPlay);


        }
        private IEnumerator BringRadiationBackResponse(PhaseChangeAction phaseChange)
        {
            bool tryPlaying = true;
            string message = base.Card.Title + " plays a radiation card from the Trash.";
            if (!base.TurnTaker.Trash.Cards.Any((Card c) => c.DoKeywordsContain("radiation")))
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