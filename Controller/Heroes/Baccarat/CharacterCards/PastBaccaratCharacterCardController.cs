using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Baccarat
{
    public class PastBaccaratCharacterCardController : HeroCharacterCardController
    {
        public PastBaccaratCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now.
                        IEnumerator coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //One player may use a power now.
                        IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine3 = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Play or draw a card. If you played a trick this way, draw a card.
            List<PlayCardAction> playedCard = new List<PlayCardAction>();
            IEnumerable<Function> functionChoices = new Function[]
            {
				//Play a card...
				new Function(base.HeroTurnTakerController, "Play a card", SelectionType.PlayCard, () => base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, playedCard, cardSource: base.GetCardSource())),

				//...or draw a card.
				new Function(base.HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => base.DrawCard())
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If you played a trick this way, draw a card.
            if (playedCard.Any() && playedCard.FirstOrDefault().WasCardPlayed && playedCard.FirstOrDefault().CardToPlay.DoKeywordsContain("trick"))
            {
                coroutine = base.DrawCard();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}