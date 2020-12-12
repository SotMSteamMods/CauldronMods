using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Impact
{
    public class ImpactCharacterCardController : HeroCharacterCardController
    {
        public ImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} deals 1 target 1 infernal damage. You may destroy 1 hero ongoing card to increase this damage by 2."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One hero may use a power now.",
                        yield break;
                    }
                case 1:
                    {
                        //"Select a hero target. That target deals 1 other target 1 projectile damage.",
                        break;
                    }
                case 2:
                    {
                        //"Damage dealt to environment cards is irreducible until the start of your turn."
                        break;
                    }
            }
            yield break;
        }
    }
}