using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    class PastMagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
    {
        public PastMagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"One player may play a relic card. Another may move a relic from their trash to the top of their deck. If neither happens, draw a card."

            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case (0):
                    {
                        //"One target deals another target 1 melee damage.",
                        break;
                    }
                case (1):
                    {
                        //"Put a card in a trash pile under its associated deck.",

                        break;
                    }
                case (2):
                    {
                        //"One player selects a keyword and reveals the top 3 cards of their deck, putting any revealed cards with that keyword into their hand and discarding the rest."
                        break;
                    }
            }
            yield break;
        }
    }
}
