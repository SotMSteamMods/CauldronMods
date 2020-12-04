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

        public override IEnumerator StartGame()
        {
            if (!(base.CharacterCardController is CeladrochCharacterCardController))
            {
                yield break;
            }

            //// Search the deck for 1 copy of Stained Wolf and 1 copy of Painted Viper and put them into play.
            //IEnumerator stainedWolfRoutine = this.PutCardIntoPlay(StainedWolfCardController.Identifier, shuffleDeckAfter: false);
            //IEnumerator paintedViperRoutine = this.PutCardIntoPlay(PaintedViperCardController.Identifier);

            //if (base.UseUnityCoroutines)
            //{
            //    yield return base.GameController.StartCoroutine(stainedWolfRoutine);
            //    yield return base.GameController.StartCoroutine(paintedViperRoutine);
            //}
            //else
            //{
            //    base.GameController.ExhaustCoroutine(stainedWolfRoutine);
            //    base.GameController.ExhaustCoroutine(paintedViperRoutine);
            //}
        }
    }
}
