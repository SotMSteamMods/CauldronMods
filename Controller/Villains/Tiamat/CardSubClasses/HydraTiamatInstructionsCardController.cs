using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class HydraTiamatInstructionsCardController : TiamatSubCharacterCardController
    {
        public HydraTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected abstract ITrigger[] AddFrontTriggers();
        protected abstract ITrigger[] AddFrontAdvancedTriggers();
        protected abstract ITrigger[] AddBackTriggers();

        protected CardController firstHead;
        protected CardController secondHead;
        protected string element;
        protected IEnumerator alternateElementCoroutine;

        public override void AddSideTriggers()
        {
            //Front Triggers
            if (!base.Card.IsFlipped)
            {
                //At the start of the villain turn, if firstHead is decapitated, flip this card and put secondHead into play with 15 HP.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker && this.firstHead.Card.IsFlipped, this.FlipThisCardResponse, TriggerType.FlipCard));
                base.AddSideTriggers(this.AddFrontTriggers());
                if (Game.IsAdvanced)
                {
                    base.AddSideTriggers(this.AddFrontAdvancedTriggers());
                }
            }
            else //Back Triggers
            {
                //Each card has an alternate response if its firstHead's element enters play when the firstHead is incap'd
                base.AddSideTrigger(base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.WasCardPlayed && action.CardToPlay.Identifier == this.element && this.firstHead.Card.IsFlipped && !this.secondHead.Card.IsFlipped, (PlayCardAction action) => this.AlternateElementResponse(action), TriggerType.DealDamage, TriggerTiming.After));
                base.AddSideTriggers(this.AddBackTriggers());
            };
        }

        private IEnumerator FlipThisCardResponse(PhaseChangeAction action)
        {
            //At the start of the villain turn, if firstHead is decapitated, flip this card and put secondHead into play with 15 HP.
            IEnumerator coroutine = base.GameController.MoveIntoPlay(base.TurnTakerController, secondHead.Card, base.TurnTaker, base.GetCardSource());
            IEnumerator coroutine2 = base.GameController.FlipCard(secondHead, cardSource: base.GetCardSource()); ;
            IEnumerator coroutine3 = base.GameController.MakeTargettable(secondHead.Card, 15, 15, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
                base.GameController.ExhaustCoroutine(coroutine3);
            }

            coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
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

        //Did Head Deal Damage This Turn
        protected bool DidDealDamageThisTurn(Card card)
        {
            int result = 0;
            try
            {
                result = (from e in base.GameController.Game.Journal.DealDamageEntriesThisTurn()
                          where e.SourceCard == card
                          select e.Amount).Sum();
            }
            catch (OverflowException ex)
            {
                Log.Warning("DamageDealtThisTurn overflowed: " + ex.Message);
                result = int.MaxValue;
            }
            return result != 0;
        }
        private IEnumerator AlternateElementResponse(PlayCardAction action)
        {
            //Whenever the corresponding "Element of" Card enters play and the firstHead is decapitated, if the secondHead is active do the alternate response.
            IEnumerator coroutine = alternateElementCoroutine;
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

        protected int PlusNumberOfACardInTrash(int value, string identifier)
        {
            return value + (from card in base.TurnTaker.Trash.Cards
                            where card.Identifier == identifier
                            select card).Count<Card>();
        }
    }
}