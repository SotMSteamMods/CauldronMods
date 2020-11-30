using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class MisbehaviorCardController : CardController
    {
        public MisbehaviorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
            List<SelectNumberDecision> storedNumber = new List<SelectNumberDecision>();
            //Reveal up to 3 
            IEnumerator coroutine = base.GameController.SelectNumber(this.DecisionMaker, SelectionType.DealDamage, 0, 3, storedResults: storedNumber, cardSource: base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            SelectNumberDecision selectNumberDecision = storedNumber.FirstOrDefault<SelectNumberDecision>();
            //...cards from the top of your deck. Put 1 of them into play or into your hand. Put the rest into your trash.
            coroutine = base.RevealCards_SelectSome_MoveThem_DiscardTheRest(base.HeroTurnTakerController, base.TurnTakerController, base.TurnTaker.Deck, (Card c) => true, selectNumberDecision.SelectedNumber ?? 0, 1, false, false, true, "revealed card");
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Titan} regains X HP, where X is 3 minus the number of cards revealed this way.
            coroutine = base.GameController.GainHP(base.CharacterCard, 3 - selectNumberDecision.SelectedNumber, cardSource: base.GetCardSource());
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