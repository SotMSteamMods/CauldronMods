using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{

    public class CeladrochTurnTakerController : TurnTakerController
    {
        public CeladrochTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        /* Setup:
         * At the start of the game, put {Celadroch}'s villain character cards into play, 'Black Wind Rising' side up.
		 * Search the villain deck for 3 relic cards and put them into play. Shuffle the villain deck.
		 * Flip the top card of the villain deck face up."
         */

        public override IEnumerator StartGame()
        {
            if (!(base.CharacterCardController is CeladrochCharacterCardController))
            {
                yield break;
            }

            IEnumerator coroutine = PutCardsIntoPlay(new LinqCardCriteria(c => c.Owner == TurnTaker && c.IsRelic, "relic"), 3);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var topCard = TurnTaker.Deck.TopCard;
            var cc = FindCardController(topCard);
            cc.SetCardProperty("CeladrochsTopCard", true);

            coroutine = GameController.SendMessageAction($"Celadroch's top card is {topCard.Title}", Priority.High, CharacterCardController.GetCardSource(), new[] { topCard });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.TurnTaker.IsChallenge)
            {
                //At the start of the game, put a token in the storm pool.
                base.GameController.AddTokensToPool(base.CharacterCard.FindTokenPool("StormPool"), 1, base.CharacterCardController.GetCardSource());
            }
        }
    }
}
