using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Cauldron.Celadroch
{
    public class CeladrochCharacterCardController : VillainCharacterCardController
    {
        /*
         
         *
         */



        public CeladrochCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddSideTriggers()
        {
            // Front side (Mural of the Forest)
            if (!base.Card.IsFlipped)
            {

            }
            // Flipped side (Unmarked Dryad)
            else
            {

                if (this.IsGameAdvanced)
                {

                }

                base.AddDefeatedIfDestroyedTriggers();
            }
        }

        public override bool CanBeDestroyed => base.CharacterCard.IsFlipped;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (base.Card.IsFlipped)
            {
                yield break;
            }


        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {


            yield break;
        }

    }
}