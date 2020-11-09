using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class VortexGlitchCardController : CardController
    {
        #region Constructors

        public VortexGlitchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Players may not play one-shots.
            base.CannotPlayCards((TurnTakerController turnTakerController) => turnTakerController.TurnTaker != null && turnTakerController.IsHero, (Card c) => c.IsHero && c.IsOneShot);
            //When another environment card enters play, destroy this card.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction p) => p.CardEnteringPlay.IsEnvironment, (base.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After);
        }

        #endregion Methods
    }
}