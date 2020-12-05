using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cypher
{
    public class CypherCharacterCardController : CypherBaseCharacterCardController
    {

        public CypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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