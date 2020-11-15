using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class PrismaticVisionCardController : MalichaeCardController
    {
        public PrismaticVisionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            var coroutine = base.RevealCardsFromTopOfDeck_DetermineTheirLocation(DecisionMaker, DecisionMaker, DecisionMaker.HeroTurnTaker.Deck,
                                    new MoveCardDestination(DecisionMaker.HeroTurnTaker.Hand),
                                    new MoveCardDestination(DecisionMaker.HeroTurnTaker.Trash),
                                    numberOfReveals: 3,
                                    numberToSelect: 2,
                                    numberToReturn: 0);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.SelectAndPlayCardFromHand(DecisionMaker,
                                cardCriteria: new LinqCardCriteria(c => IsDjinn(c), "djinn"));
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
