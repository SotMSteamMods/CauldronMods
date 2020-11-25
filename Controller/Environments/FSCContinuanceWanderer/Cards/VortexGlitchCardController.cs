using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class VortexGlitchCardController : CardController
    {

        public VortexGlitchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override void AddTriggers()
        {
            //Players may not play one-shots.
            base.CannotPlayCards(cardCriteria: (Card c) => c.IsHero && c.IsOneShot);
            //When another environment card enters play, destroy this card.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction p) => p.CardEnteringPlay.IsEnvironment && p.CardEnteringPlay.Identifier != base.Card.Identifier, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

    }
}
