using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class HarbingerQuicksilverCharacterCardController : HeroCharacterCardController
    {
        public HarbingerQuicksilverCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        break;
                    }
                case 1:
                    {
                        //"Destroy a target with exactly 3HP.",
                        break;
                    }
                case 2:
                    {
                        //"One player may discard a one-shot. If they do, 2 players may each draw a card now."
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Discard the top 2 cards of your deck, or put 1 card fomr your trash into your hand."
            yield break;
        }

    }
}