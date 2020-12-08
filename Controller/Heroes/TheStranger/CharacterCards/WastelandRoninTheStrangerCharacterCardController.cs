using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class WastelandRoninTheStrangerCharacterCardController : HeroCharacterCardController
    {
        public WastelandRoninTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //Discard a rune. If you do, {TheStranger} deals 1 target 4 infernal damage.
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        break;
                    }
                case 1:
                    {
                        //One hero target deals itself 1 infernal damage.
                        break;

                    }
                case 2:
                    {
                        //Destroy all environment cards. Play the top card of the environment deck.
                        break;
                    }
            }
            yield break;
        }

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }

    }
}
