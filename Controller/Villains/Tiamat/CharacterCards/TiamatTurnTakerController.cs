using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Tiamat
{
    public class TiamatTurnTakerController : TurnTakerController
    {
        public TiamatTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            if (!(base.CharacterCardController is WinterTiamatCharacterCardController))
            {
                Card earth = base.TurnTaker.GetCardByIdentifier("EarthTiamatCharacter");
                Card wind = base.TurnTaker.GetCardByIdentifier("WindTiamatCharacter");
                Card decay = base.TurnTaker.GetCardByIdentifier("DecayTiamatCharacter");
                var a = base.CharacterCardController;
            }
            else
            {
                var a = base.CharacterCardController;
            }
            yield break;
        }
    }
}