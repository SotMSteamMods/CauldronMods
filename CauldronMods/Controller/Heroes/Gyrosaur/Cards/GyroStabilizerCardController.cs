using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public class GyroStabilizerCardController : GyrosaurUtilityCardController
    {
        public GyroStabilizerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ActivatesEffects);
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, discard up to 3 cards. Draw as many cards as you discarded this way.",
            yield break;
        }

        public override bool? AskIfActivatesEffect(TurnTakerController turnTakerController, string effectKey)
        {
            //"Whenever you evaluate the number of Crash cards in your hand, you may treat it as being 1 higher or 1 lower than it is."
            //Log.Debug("Gyro Stabilizer is being asked whether it can activate an effect...");
            if (turnTakerController == this.TurnTakerController && effectKey == StabilizerKey)
            {
                return true;
            }
            return null;
        }
    }
}
