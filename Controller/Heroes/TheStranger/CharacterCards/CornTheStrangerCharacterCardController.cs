using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class CornTheStrangerCharacterCardController : HeroCharacterCardController
    {
        public CornTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //{TheStranger} or a target next to a Rune deals 1 target 2 psychic damage.
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
                        //One hero may use a power now.
                        break;

                    }
                case 2:
                    {
                        //One target deals itself 1 irreducible toxic damage.
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
