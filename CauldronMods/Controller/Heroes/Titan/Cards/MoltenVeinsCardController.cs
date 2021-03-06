using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Titan
{
    public class MoltenVeinsCardController : TitanCardController
    {
        public MoltenVeinsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
            //{Titan} regains 2HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 2, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.Custom, base.GetTitanform(), storedResults: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(storedResults))
            {
                Location titanformLocation = base.GetTitanform().Location;
                bool searchDeck = false;
                bool searchTrash = false;
                if (titanformLocation == base.TurnTaker.Deck)
                {
                    searchDeck = true;
                }
                else if (titanformLocation == base.TurnTaker.Trash)
                {
                    searchTrash = true;
                }

                //You may search your deck and trash for a copy of the card Titanform and put it into your hand. If you searched your deck, shuffle it.
                coroutine = base.SearchForCards(base.HeroTurnTakerController, searchDeck, searchTrash, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "Titanform"), false, true, false, shuffleAfterwards: searchDeck);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //You may play a card.
            coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController);
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to put Titanform in your hand?", "Should they put Titanform in their hand?", "Vote for if they should put Titanform in their hand?", "put Titanform in hand");

        }
    }
}