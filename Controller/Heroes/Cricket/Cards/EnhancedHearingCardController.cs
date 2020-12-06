using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class EnhancedHearingCardController : CardController
    {
        public EnhancedHearingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of your turn, reveal the top card of 2 different decks, then replace them.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.RevealCardsResponse, TriggerType.RevealCard);
            //Increase sonic damage dealt to {Cricket} by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.Target == base.CharacterCard && action.DamageType == DamageType.Sonic, 1);
        }

        private IEnumerator RevealCardsResponse(PhaseChangeAction action)
        {
            //...reveal the top card of 2 different decks, then replace them.
            List<SelectLocationDecision> storedResult = new List<SelectLocationDecision>();
            //Pick first deck
            IEnumerator coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, null, storedResult, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResult != null && storedResult.FirstOrDefault().SelectedLocation.Location != null)
            {
                //Reveal top card and maybe discard
                Location selectedDeck = storedResult.FirstOrDefault().SelectedLocation.Location;
                coroutine = base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.FindTurnTakerController(selectedDeck.OwnerTurnTaker), selectedDeck, false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Second Deck
                coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => deck != selectedDeck, storedResult, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (storedResult != null && storedResult.LastOrDefault().SelectedLocation.Location != null)
                {
                    //Reveal top card and maybe discard
                    selectedDeck = storedResult.LastOrDefault().SelectedLocation.Location;
                    coroutine = base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.FindTurnTakerController(selectedDeck.OwnerTurnTaker), selectedDeck, false);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }
    }
}