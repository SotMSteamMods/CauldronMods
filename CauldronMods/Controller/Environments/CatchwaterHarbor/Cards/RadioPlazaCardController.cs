using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class RadioPlazaCardController : CatchwaterHarborUtilityCardController
    {
        public RadioPlazaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"The top card of {TurnTaker.Deck.GetFriendlyName()} is {TurnTaker.Deck.TopCard.Title}.").Condition = () => Card.IsInPlayAndHasGameText && TurnTaker.Deck.HasCards;
        }

        public override void AddStartOfGameTriggers()
        {
            BuildTopDeckSpecialStrings();
        }

        private void BuildTopDeckSpecialStrings()
        {
            //this needs to be all turntakers in all zones.
            IEnumerable<TurnTaker> activeTurnTakers =  FindTurnTakersWhere((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && !tt.IsEnvironment, true);
            foreach(TurnTaker tt in activeTurnTakers)
            {
                foreach(Location deck in tt.Decks.Where(deck => deck.IsRealDeck))
                {
                    var ss = SpecialStringMaker.ShowSpecialString(() => $"The top card of {deck.GetFriendlyName()} is {deck.TopCard.Title}.", relatedCards: () => tt.CharacterCards.Where(c => c.IsInPlayAndHasGameText));
                    ss.Condition = () => Card.IsInPlayAndHasGameText && deck.HasCards && GameController.IsLocationVisibleToSource(deck, GetCardSource());
                }
            }
        }

        public override void AddTriggers()
        {
            //Damage dealt to hero targets is irreducible.
            AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => IsHeroTarget(dd.Target) && GameController.IsCardVisibleToCardSource(dd.Target, GetCardSource()));
            //At the start of the environment turn, destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
