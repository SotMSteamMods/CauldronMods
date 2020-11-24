using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Achievements;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsTurnTakerController : TurnTakerController
    {
        public StSimeonsCatacombsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            //Find all rooms in the deck
            List<Card> rooms = (from c in base.TurnTaker.GetAllCards(true)
                                where c.IsRoom && !c.Location.IsOutOfGame
                                select c).ToList();

            Card catacombs = base.TurnTaker.FindCard(StSimeonsCatacombsCardController.Identifier);
            IEnumerator playCatacombs = base.GameController.PlayCard(this, catacombs);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCatacombs);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCatacombs);
            }

            CardController cardController = base.GameController.FindCardController(catacombs);

            //move all room cards to under the catacombs card
            //shuffle the cards under the catacombs card
            IEnumerator coroutine = base.GameController.BulkMoveCards(this, rooms, catacombs.UnderLocation, cardSource: cardController.GetCardSource());
            IEnumerator coroutine2 = base.GameController.ShuffleLocation(catacombs.UnderLocation);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }
    }
}
