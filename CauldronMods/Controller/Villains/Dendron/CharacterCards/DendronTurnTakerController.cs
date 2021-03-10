using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{

    public class DendronTurnTakerController : TurnTakerController
    {
        public DendronTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            if (base.CharacterCardController is DendronCharacterCardController)
            {
                // Search the deck for 1 copy of Stained Wolf and 1 copy of Painted Viper and put them into play.
                IEnumerator stainedWolfRoutine = this.PutCardIntoPlay(StainedWolfCardController.Identifier, shuffleDeckAfter: false);
                IEnumerator paintedViperRoutine = this.PutCardIntoPlay(PaintedViperCardController.Identifier);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(stainedWolfRoutine);
                    yield return base.GameController.StartCoroutine(paintedViperRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(stainedWolfRoutine);
                    base.GameController.ExhaustCoroutine(paintedViperRoutine);
                }
            }
            if (base.CharacterCardController is WindcolorDendronCharacterCardController)
            {
                /*
                 * At the start of the game, put {Dendron}'s villain character cards into play, 'Outside The Lines' side up.
                 * Search the deck for all copies of Painted Viper and Stained Wolf. Place them beneath this card and shuffle the villain deck.
                 */

                var cardsToPutUnderDendron = FindCardsWhere(c => c.Identifier == "StainedWolf" || c.Identifier == "PaintedViper");

                if(GameController.Game.IsChallenge)
                {
                    // put 6 random tattoos under Dendron instead
                    cardsToPutUnderDendron = FindCardsWhere(c => c.DoKeywordsContain("tattoo") && c.Owner == TurnTaker).OrderBy(c => GameController.Game.RNG.Next()).Take(6);
                }

                var coroutine = GameController.MoveCards(this, cardsToPutUnderDendron, CharacterCard.UnderLocation, cardSource: CharacterCardController.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.ShuffleLocation(TurnTaker.Deck);
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
    }
}
