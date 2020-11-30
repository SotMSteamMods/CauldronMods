using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class MoltenVeinsCardController : CardController
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

            //You may search your deck and trash for a copy of the card Titanform and put it into your hand. If you searched your deck, shuffle it.
            coroutine = base.SearchForCards(base.HeroTurnTakerController, true, false, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "Titanform"), false, true, false, true, shuffleAfterwards: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
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
    }
}