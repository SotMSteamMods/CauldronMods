using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Achievements;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class TheChasmOfAThousandNightsTurnTakerController : TurnTakerController
    {
        public TheChasmOfAThousandNightsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            List<Card> natures = (from c in TurnTaker.GetAllCards(true)
                                  where IsNature(c) && !c.Location.IsOutOfGame
                                  select c).ToList();
            List<CardController> natureCC = new List<CardController>();
            foreach (Card item in natures)
            {
                natureCC.Add(FindCardController(item));
            }
            Card chasm = TurnTaker.FindCard(TheChasmOfAThousandNightsCardController.Identifier, realCardsOnly: false);
            IEnumerator playChasm = GameController.PlayCard(this, chasm, isPutIntoPlay: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playChasm);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playChasm);
            }

            CardController chasmCardController = base.GameController.FindCardController(chasm);

            //move all nature cards to under the chasm card face down
            //shuffle the cards under the chasm card
            IEnumerator coroutine = GameController.BulkMoveCards(this, natures, chasm.UnderLocation, cardSource: chasmCardController.GetCardSource());
            IEnumerator coroutine3 = GameController.ShuffleLocation(TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine3);
            }

            yield break;
        }

        protected bool IsNature(Card card)
        {
            return card.DoKeywordsContain(NatureKeyword);
        }

        public static readonly string NatureKeyword = "nature";

    }
}
