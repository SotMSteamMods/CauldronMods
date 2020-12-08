using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class PastTheStrangerCharacterCardController : HeroCharacterCardController
    {
        public PastTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //The next time {TheStranger} would deal himself damage, redirect it to another target.
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may use a power now.
                        break;
                    }
                case 1:
                    {
                        //Destroy 1 ongoing card.
                        break;

                    }
                case 2:
                    {
                        //The target with the lowest HP deals itself 1 irreducible toxic damage.
                        break;
                    }
            }
            yield break;
        }

    }
}
