using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using CauldronMods.Controller.Villains.Vector.CardSubClasses;


namespace Cauldron.Vector
{
    public class VectorCharacterCardController : VectorBaseCardController
    {
        /*
         *
         * Setup:
         *
         * At the start of the game, put {Vector}'s villain character cards into play, "Asymptomatic Carrier" side up, with 40 HP.
         *
         * Gameplay:
         *
         * Whenever {Vector} is dealt damage, play the top card of the villain deck.
         * If {Vector} regains all his HP, he escapes. Game over.
         * Supervirus is indestructible. Cards beneath it are indestructible and have no game text.
         * Whenever Supervirus is in play and there are {H + 2} or more cards beneath it, flip {Vector}'s villain character cards.
         * If {Vector} is dealt damage by an environment card, he becomes immune to damage dealt by environment cards until the end of the turn.
         *
         * Advanced:
         *
         * At the end of the villain turn, {Vector} regains 2 HP.
         *
         * Flipped Gameplay:
         *
         * Whenever {Vector} flips to this side, remove Supervirus from the game. Put all virus cards that were beneath it into the villain trash.
         * Reduce damage dealt to {Vector} by 1 for each villain target in play.
         * At the end of the villain turn, play the top card of the villain deck.
         *
         * Flipped Advanced:
         *
         * Increase damage dealt by {Vector} by 2.
         *
         *
         */

        private const int AdvancedHpGain = 2;
        private const int AdvancedDamageIncrease = 2;

        public VectorCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {

            yield break;
        }

    }
}