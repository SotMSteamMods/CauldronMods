using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class PhaseCharacterCardController : VillainCharacterCardController
    {
        public PhaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                //Front
                //At the start of the villain turn, if there are 3 or more obstacles in play, flip {Phase}'s villain character cards.
                //Increase damage dealt by {Phase} by 1 for each obstacle that has been removed from the game.
                //Phase is immune to damage dealt by environment cards.
                //At the end of the villain turn, play the top card of the villain deck.
                if (base.Game.IsAdvanced)
                {
                    //Front - Advanced
                    //When {Phase} is damaged, she becomes immune to damage until the end of the turn.
                }
            }
            else
            {
                //Back

                //At the end of the villain turn, {Phase} deals each hero target {H} radiant damage. Then, flip {Phase}'s villain character cards.
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            if (base.Card.IsFlipped)
            {
                //Back
                //When {Phase} flips to this side, destroy the obstacle with the lowest HP and remove it from the game. If the card Insubstantial Matador is in play, destroy it.
                if (base.Game.IsAdvanced)
                {
                    //Back - Advanced
                    //When {Phase} flips to this side, destroy {H - 2} hero ongoing cards.
                }
            }
            yield break;
        }
    }
}