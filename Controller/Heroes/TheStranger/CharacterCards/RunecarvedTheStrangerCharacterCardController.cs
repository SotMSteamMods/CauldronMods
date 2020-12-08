using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class RunecarvedTheStrangerCharacterCardController : HeroCharacterCardController
    {
        public RunecarvedTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //1 hero target regains 2 HP.
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now.
                        break;
                    }
                case 1:
                    {
                        //Destroy 1 hero ongoing, 1 non-hero ongoing, and 1 environment card.
                        break;

                    }
                case 2:
                    {
                        //Villain targets may not regain HP until the start of your turn.
                        break;
                    }
            }
            yield break;
        }

    }
}
