using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Titan
{
    public class TitanTurnTakerController : HeroTurnTakerController
    {
        public TitanTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            var cards = TurnTaker.GetCardsWhere(c => c.IsCharacter && c.SharedIdentifier != null && c.SharedIdentifier != CharacterCard.SharedIdentifier).ToList();
            var coroutine = GameController.BulkMoveCards(this, cards, TurnTaker.InTheBox, performBeforeDestroyActions: false); //send the other promo's to the shadowrealm
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.StartGame();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

        }
    }
}
