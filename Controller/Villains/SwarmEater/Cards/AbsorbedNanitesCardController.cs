using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class AbsorbedNanitesCardController : CardController
    {
        public AbsorbedNanitesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //This card and cards beneath it are indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card and cards beneath it are indestructible
            return card == base.Card || card.Location == base.Card.UnderLocation;
        }

        public override void AddTriggers()
        {
            //Whenever {SwarmEater} destroys a villain target, put it beneath this card. 
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.CardToDestroy.Card.IsVillain && action.ResponsibleCard == base.CharacterCard, this.DestroyVillainResponse, TriggerType.MoveCard, TriggerTiming.After);
            //Activate the Absorb text of all cards beneath this one. Ignore all other game text on those cards.
            base.AddTrigger<MoveCardAction>((MoveCardAction action) => action.Destination == base.Card.UnderLocation, this.AbsorbResponse, TriggerType.AddTrigger, TriggerTiming.After);
        }

        private IEnumerator DestroyVillainResponse(DestroyCardAction action)
        {
            //...put it beneath this card.
            action.SetPostDestroyDestination(base.Card.UnderLocation, cardSource: base.GetCardSource());
            action.PreventRemoveTriggers = true;
            yield break;
        }

        private IEnumerator AbsorbResponse(MoveCardAction action)
        {
            CardController absorbed = base.FindCardController(action.CardToMove);
            absorbed.RemoveAllTriggers();
            absorbed.AddAllTriggers();
            yield break;
        }
    }
}