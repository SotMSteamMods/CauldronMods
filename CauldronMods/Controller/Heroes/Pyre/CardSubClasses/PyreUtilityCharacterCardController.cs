using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCharacterCardController : HeroCharacterCardController
    {
        private const string LocationKnown = "CascadeLocationKnownKey";

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is PyreTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

        protected enum CustomMode
        {
            CardToIrradiate,
            PlayerToIrradiate,
            Unique
        }

        protected CustomMode CurrentMode;

        protected PyreUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildCascadeLocationString()).Condition = () => TurnTakerController is PyreTurnTakerController;
            SpecialStringMaker.ShowSpecialString(() => BuildListIrradiatedHeroes()).ShowWhileIncapacitated = true;

            SpecialStringMaker.ShowSpecialString(() => "You may need to right click {Rad} cards and select 'Play Card' in order to play them.", showInEffectsList: () => true, relatedCards: () => FindCardsWhere(c => c.IsIrradiated())).Condition = () => FindCardsWhere(c => c.IsIrradiated()).Any();

        }

        private string BuildListIrradiatedHeroes()
        {
            IEnumerable<TurnTaker> irradiatedHeroes = GameController.GetAllCards().Where((Card c) => c.IsIrradiated() && c.Location.IsHand).Select(c => c.Owner).Distinct();
            if(!irradiatedHeroes.Any())
            {
               return $"No heroes have {PyreExtensionMethods.Irradiated} cards in their hand.";
            }

            return $"Heroes with {PyreExtensionMethods.Irradiated} cards in hand: {irradiatedHeroes.Select(tt => tt.NameRespectingVariant).ToRecursiveString()}";
        }

        public override void AddStartOfGameTriggers()
        {

            if (TurnTakerController is PyreTurnTakerController ttc)
            {
                AddTrigger((GameAction ga) => !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
                ttc.MoveMarkersToSide();
            }
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            AddTrigger((MoveCardAction mc) => mc.CardToMove.IsByIrradiationMarker() && (mc.Origin.IsHand || mc.Origin.IsRevealed) && !(mc.Destination.IsHand || mc.Destination.IsRevealed), mc => this.ClearIrradiation(mc.CardToMove), TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
            AddTrigger((PlayCardAction pc) => pc.CardToPlay.IsByIrradiationMarker(), pc => this.ClearIrradiation(pc.CardToPlay), TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
            AddTrigger((BulkMoveCardsAction bmc) => !(bmc.Destination.IsHand || bmc.Destination.IsRevealed) && bmc.CardsToMove.Any(c => c.IsByIrradiationMarker()), CleanUpBulkIrradiated, TriggerType.Hidden, TriggerTiming.After, ignoreBattleZone: true);
        }

        protected IEnumerator CleanUpBulkIrradiated(BulkMoveCardsAction bmc)
        {
            foreach(Card c in bmc.CardsToMove)
            {
                if(c.IsByIrradiationMarker())
                {
                    IEnumerator coroutine = this.ClearIrradiation(c);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }
        protected IEnumerator SelectAndIrradiateCardsInHand(HeroTurnTakerController decisionMaker, TurnTaker playerWithHand, int maxCards, int? minCards = null, List<SelectCardDecision> storedResults = null, Func<Card, bool> additionalCriteria = null)
        {

            if (additionalCriteria == null)
            {
                additionalCriteria = (Card c) => true;
            }
            Func<Card, bool> handCriteria = (Card c) => c != null && c.IsInHand;
            if (playerWithHand != null)
            {
                handCriteria = (Card c) => c != null && c.Location == playerWithHand.ToHero().Hand;
            }

            var fullCriteria = new LinqCardCriteria((Card c) => handCriteria(c) && !c.IsIrradiated() && additionalCriteria(c), $"non-{PyreExtensionMethods.Irradiated}");
            if (storedResults == null)
            {
                storedResults = new List<SelectCardDecision>();
            }
            if (minCards == null)
            {
                minCards = maxCards;
            }

            var oldMode = CurrentMode;
            CurrentMode = CustomMode.CardToIrradiate;
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(decisionMaker, fullCriteria, SelectionType.Custom, c => this.IrradiateCard(c), maxCards, false, minCards, storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            CurrentMode = oldMode;
            yield break;
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            SetOverrideTurnTakerIrradiatedMarkers();
            return base.AfterFlipCardImmediateResponse();
        }

        public void SetOverrideTurnTakerIrradiatedMarkers()
        {
            if (!GameController.Game.IsOblivAeonMode)
            {
                return;
            }

            var allMarkers = TurnTaker.GetAllCards(false).Where((Card c) => !c.IsRealCard && c.Identifier == "IrradiatedMarker");
            foreach (Card marker in allMarkers)
            {
                GameController.AddCardPropertyJournalEntry(marker, "OverrideTurnTaker", new string[] { "Cauldron.Pyre", marker.Identifier });
            }
        }

        private string BuildCascadeLocationString()
        {
            var cascades = TurnTaker.GetAllCards().Where((Card c) => c.Identifier == "RogueFissionCascade").ToList();
            if(cascades.Count() != 2)
            {
                return "Something strange happened with the number of Cascades.";
            }

            var descriptor1 = CascadeDescriptor(cascades[0]);
            var descriptor2 = CascadeDescriptor(cascades[1]);

            string start;
            if(descriptor1 == descriptor2)
            {
                start = "Both Rogue Fission Cascades are ";
                if(descriptor1 == "in an unknown location.")
                {
                    return start + "in unknown locations.";
                }
                return start + descriptor1;
            }

            start = "One Rogue Fission Cascade is ";
            return start + descriptor1 + "\n" + start + descriptor2;
        }

        private string CascadeDescriptor(Card c)
        {
            if(GameController.GetCardPropertyJournalEntryBoolean(c, LocationKnown) != true)
            {
                return "in an unknown location.";
            }

            if(c.Location.IsRevealed)
            {
                return "being revealed";
            }
            if(c.Location.IsOutOfGame)
            {
                return "removed from the game";
            }
            if(!c.Location.IsDeck)
            {
                return $"in {c.Location.GetFriendlyName()}";
            }
            else
            {
                int position = c.Location.Cards.Reverse().ToList().IndexOf(c) + 1;
                string positionString = "the ";
                if (position == 1)
                {
                    positionString += "top ";
                }
                else
                {
                    positionString += position.ToString();
                    if (position == 2)
                    {
                        positionString += "nd ";
                    }
                    else if (position == 3)
                    {
                        positionString += "rd ";
                    }
                    else
                    {
                        positionString += "th ";
                    }
                }

                positionString += $"card of {c.Location.GetFriendlyName()}.";
                return positionString;
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string radIcon = "{{Rad}}";
            if (CurrentMode is CustomMode.CardToIrradiate)
            {
                return new CustomDecisionText($"Select a card to {radIcon}", $"deciding which card to {radIcon}.", $"Vote for which card to {radIcon}", $"card to {radIcon}");
            }
            else if (CurrentMode is CustomMode.PlayerToIrradiate)
            {
                return new CustomDecisionText($"Select a player to {radIcon} a card in their hand.", $"deciding whose hand to {radIcon} cards in.", $"Vote for which player's hand to {radIcon} cards from.", $"player's hand to {radIcon} cards from");
            }
            else if (CurrentMode is CustomMode.Unique)
            {
                return new CustomDecisionText($"Select a player to {radIcon} a card in their hand.", $"deciding whose hand to {radIcon} cards in.", $"Vote for which player's hand to {radIcon} cards from.", $"player's hand to {radIcon} cards from");
            }

            return base.GetCustomDecisionText(decision);

        }
    }
}
