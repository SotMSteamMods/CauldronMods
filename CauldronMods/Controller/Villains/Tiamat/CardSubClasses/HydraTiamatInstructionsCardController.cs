using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Tiamat
{
    public abstract class HydraTiamatInstructionsCardController : TiamatSubCharacterCardController
    {
        protected HydraTiamatInstructionsCardController(Card card, TurnTakerController turnTakerController, string firstHead, string secondHead, string element) : base(card, turnTakerController)
        {
            FirstHead = firstHead;
            SecondHead = secondHead;
            Element = element;
            IsAddingHead = false;
            SpecialStringMaker.ShowIfElseSpecialString(() => FirstHeadCardController().Card.IsFlipped, () => FirstHeadCardController().Card.Title + " is decapitated.", () => FirstHeadCardController().Card.Title + " is not decapitated.").Condition = () => !base.Card.IsFlipped;
        }

        protected abstract ITrigger[] AddFrontTriggers();
        protected abstract ITrigger[] AddFrontAdvancedTriggers();
        protected abstract ITrigger[] AddBackTriggers();
        protected virtual IEnumerator alternateElementCoroutine => DoNothing();

        public string FirstHead { get; }
        public string SecondHead { get; }
        public string Element { get; }

        public bool IsAddingHead;

        public CardController FirstHeadCardController()
        {
            return base.GameController.FindCardController(FirstHead);
        }
        public CardController SecondHeadCardController()
        {
            return base.GameController.FindCardController(SecondHead);
        }

        public override void AddSideTriggers()
        {
            //Front Triggers
            if (!base.Card.IsFlipped)
            {
                //At the start of the villain turn, if firstHead is decapitated, flip this card and put secondHead into play with 15 HP.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker && this.FirstHeadCardController().Card.IsFlipped, this.FlipThisCardResponse, TriggerType.FlipCard));
                base.AddSideTriggers(this.AddFrontTriggers());
                if (Game.IsAdvanced)
                {
                    base.AddSideTriggers(this.AddFrontAdvancedTriggers());
                }
            }
            else //Back Triggers
            {
                //Each card has an alternate response if its firstHead's element enters play when the firstHead is incap'd
                base.AddSideTrigger(base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.WasCardPlayed && action.CardToPlay.Identifier == this.Element && this.FirstHeadCardController().Card.IsFlipped && !this.SecondHeadCardController().Card.IsFlipped, (PlayCardAction action) => this.AlternateElementResponse(action), TriggerType.DealDamage, TriggerTiming.After));
                base.AddSideTriggers(this.AddBackTriggers());
            };
        }

        private IEnumerator FlipThisCardResponse(PhaseChangeAction action)
        {
            this.IsAddingHead = true;
            //At the start of the villain turn, if firstHead is decapitated, flip this card and put secondHead into play with 15 HP.
            IEnumerator coroutine = base.GameController.MoveIntoPlay(base.TurnTakerController, this.SecondHeadCardController().Card, base.TurnTaker, base.GetCardSource());
            IEnumerator coroutine2 = base.GameController.FlipCard(this.SecondHeadCardController(), cardSource: base.GetCardSource()); ;
            IEnumerator coroutine3 = base.GameController.MakeTargettable(this.SecondHeadCardController().Card, 15, 15, base.GetCardSource());
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
            this.IsAddingHead = false;
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
            return value + GetNumberOfSpecificCardInTrash(identifier);
        }

        protected string BuildDecapitatedHeadList()
        {
            IEnumerable<Card> decappedHeads = FindCardsWhere((Card c) => c.IsFlipped && IsHead(c) && c.IsInPlayAndNotUnderCard).ToList();
            string decappedHeadsSpecial = "Decapitated heads: ";
            if (decappedHeads.Any())
            {
                decappedHeadsSpecial += decappedHeads.FirstOrDefault().Title;
                for (int i = 1; i < decappedHeads.Count(); i++)
                {
                    decappedHeadsSpecial += ", " + decappedHeads.ElementAt(i).Title;
                }
            }
            else
            {
                decappedHeadsSpecial += "None";
            }
            return decappedHeadsSpecial;
        }
    }
}