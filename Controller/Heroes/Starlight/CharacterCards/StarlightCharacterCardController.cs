using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightCharacterCardController : HeroCharacterCardController
    {
        public StarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Draw a card, or play a Constellation from your trash
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"Until the start of your next turn, prevent all damage that would be dealt to or by the target with the lowest HP.",
                        break;
                    }
                case 1:
                    {
                        //"1 player may use a power now.",
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        break;
                    }
            }


            yield break;
        }
    }
}