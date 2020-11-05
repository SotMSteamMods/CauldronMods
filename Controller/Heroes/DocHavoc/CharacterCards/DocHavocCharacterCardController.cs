using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocHavocCharacterCardController : HeroCharacterCardController
    {
        public DocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            return base.UsePower(index);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            return base.UseIncapacitatedAbility(index);
        }
    }
}
