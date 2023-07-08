using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheKnightUtilityCharacterCardController : HeroCharacterCardController
    {
        protected bool IsCoreCharacterCard = true;
        protected readonly string RoninKey = "WastelandRoninKnightOwnershipKey";
        private List<string> QualifiedRoninIdentifiers => new List<string> { "Cauldron.TheOldKnightCharacter", "Cauldron.TheYoungKnightCharacter" };
        public TheKnightUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            if (IsCoreCharacterCard && !Card.IsIncapacitatedOrOutOfGame && HeroTurnTakerController is TheKnightTurnTakerController knightTTC)
            {
                knightTTC.ManageCharactersOffToTheSide(true);
            }
        }
        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if (IsCoreCharacterCard)
            {
                AddSideTrigger(AddTrigger((MakeDecisionAction md) => IsSwapForOtherCardDecision(md), RemoveSubCharactersFromDecision, TriggerType.Hidden, TriggerTiming.After));
            }
        }
        private bool IsSwapForOtherCardDecision(MakeDecisionAction md)
        {
            if (md.Decision is SelectFromBoxDecision sfb && md.CardSource != null && IsHero(md.CardSource.Card))
            {
                //Log.Debug($"TurnTakerIdentifier: {sfb.SelectedTurnTakerIdentifier}");
                return sfb.SelectedTurnTakerIdentifier == "Cauldron.TheKnight";
            }
            return false;
        }
        private IEnumerator RemoveSubCharactersFromDecision(MakeDecisionAction md)
        {
            IEnumerator coroutine = DoNothing();
            if (md.Decision is SelectFromBoxDecision sfb && QualifiedRoninIdentifiers.Contains(sfb.SelectedIdentifier))
            {
                sfb.SelectedIdentifier = null;
                coroutine = GameController.SendMessageAction($"Sorry, {md.DecisionMaker.Name}, you can't swap in a Wasteland Ronin Knight character - it breaks things!", Priority.Medium, GetCardSource());
            }
            return coroutine;
        }

        protected Card GetKnightCardUser(Card c)
        {
            if (c == null)
            {
                return null;
            }

            if (this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards)
            {
                if (c.Location.IsNextToCard)
                {
                    return c.Location.OwnerCard;
                }

                if (c.Owner == this.TurnTaker)
                {
                    var propCard = GameController.GetCardPropertyJournalEntryCard(c, RoninKey);
                    return propCard ?? this.CharacterCard;
                }
            }

            return this.CharacterCard;
        }
    }
}