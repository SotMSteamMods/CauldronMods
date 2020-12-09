using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.TheKnight
{
    public class WastelandRoninTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public WastelandRoninTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private Card _youngKnight = null;
        private Card _oldKnight = null;

        private Card youngKnight
        {
            get
            {
                if (_youngKnight == null)
                {
                    _youngKnight = GameController.FindCard("TheYoungKnightCharacter");
                }
                return _youngKnight;
            }
        }
        private Card oldKnight
        {
            get
            {
                if (_oldKnight == null)
                {
                    _oldKnight = GameController.FindCard("TheOldKnightCharacter");
                }
                return _oldKnight;
            }
        }
        public override void AddStartOfGameTriggers()
        {
            var cards = (HeroTurnTakerController as TheKnightTurnTakerController).ManageCharactersOffToTheSide(false);

            _youngKnight = cards.Where((Card c) => c.Identifier == "TheYoungKnightCharacter").FirstOrDefault();
            _oldKnight = cards.Where((Card c) => c.Identifier == "TheOldKnightCharacter").FirstOrDefault();

            
            //"If you have no hero character targets in play, flip this card.",
			//"When 1 of your equipment cards enter play, put it next to 1 of your active knights.",
			//"When your cards refer to �The Knight�, put it next to 1 of your active knights. For equipment cards, you must choose the knight they are next to. Stalwart Shield does not reduce damage to the other knight's equipment targets.",
			//"Whenever an equipment enters play next to The Young Knight, she deals 1 target 1 toxic damage.",
			//"Whenever an equipment card enters play next to The Old Knight, draw a card."
            
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger(FlipCriteria, (GameAction ga) => base.GameController.FlipCard(FindCardController(base.Card), treatAsPlayed: false, treatAsPutIntoPlay: false, null, null, GetCardSource()), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddSideTriggers(AddTargetEntersPlayTrigger((Card c) => base.Card.IsFlipped && base.CharacterCards.Contains(c), (Card c) => base.GameController.FlipCard(FindCardController(base.Card), treatAsPlayed: false, treatAsPutIntoPlay: false, null, null, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, isConditional: false, outOfPlayTrigger: true));
            }
        }

        private bool FlipCriteria(GameAction ga)
        {
            return((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !base.Card.IsFlipped && FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c.IsHeroCharacterCard && c.IsActive && c != base.Card).Count() == 0);
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            RemoveAllTriggers();
            AddSideTriggers();
            yield return null;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"One hero deals 1 target 1 projectile damage.",
                        break;
                    }
                case 1:
                    {
                        //"One target regains 1 HP.",

                        break;
                    }
                case 2:
                    {
                        //"One player may discard a card to play 2 cards now."
                        break;
                    }
            }


            yield break;
        }
    }
}