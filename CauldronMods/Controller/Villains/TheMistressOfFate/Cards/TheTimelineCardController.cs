﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TheTimelineCardController : TheMistressOfFateUtilityCardController
    {

        /*
         *    "This card is indestructible.",
         *    "Day cards cannot be affected by non-villain cards. Cards beneath villain cards are not considered in play.",
         *    "Immediately after the environment turn, flip {TheMistressOfFate}'s villain character cards if one of these things occurs:",
         *    "{Bulletpoint} The players choose to flip her.",
         *    "{Bulletpoint} All heroes are incapacitated.",
         *    "{Bulletpoint} All Day cards are face up.",
         *    "{MistressOfFateDayCards}"
         */
        public TheTimelineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            SpecialStringMaker.ShowSpecialString(() => BuildNumberOfDaySpecialString());
        }

        public override void AddTriggers()
        {
            //"Immediately after the environment turn...",
            AddPhaseChangeTrigger(tt => true, p => true, (PhaseChangeAction pca) => pca.FromPhase != null && pca.FromPhase.IsEnvironment && !pca.ToPhase.IsEnvironment, CheckTimelineResetResponse, new TriggerType[] { TriggerType.FlipCard }, TriggerTiming.Before);

            AddTrigger((MakeDecisionsAction md) => md.CardSource != null && !md.CardSource.Card.IsVillain, RemoveDecisionsFromMakeDecisionResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
        }

        private IEnumerator CheckTimelineResetResponse(PhaseChangeAction pca)
        {
            bool shouldFlipMistress = false;
            IEnumerator coroutine;
            //...Flip {TheMistressOfFate}'s villain character cards if one of these things occurs:
            if (GameController.AllHeroes.All(htt => htt.IsIncapacitatedOrOutOfGame))
            {
                //"{Bulletpoint} All heroes are incapacitated.",
                coroutine = GameController.SendMessageAction("All heroes are incapacitated! The rest of the timeline sees no interference from them - until it begins again...", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                shouldFlipMistress = true;
            }
            else if(GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsDay(c)).Count() == 4)
            {
                //"{Bulletpoint} All Day cards are face up.",
                coroutine = GameController.SendMessageAction("The Timeline has reached its end! The world is thrown back to its beginning...", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                shouldFlipMistress = true;
            }
            else
            {
                //*"{Bulletpoint} The players choose to flip her.",
                var storedResults = new List<bool>();
                coroutine = MakeUnanimousDecision(_ => true, SelectionType.Custom, storedResults: storedResults, associatedCards: new Card[] { CharacterCard });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                shouldFlipMistress = storedResults.FirstOrDefault();
            }

            if(shouldFlipMistress)
            {
                coroutine = GameController.FlipCard(CharacterCardController, cardSource: GetCardSource(), allowBackToFront: false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        //here down sets the 'day cards cannot be affected by nonvillain cards' effect

        private IEnumerator RemoveDecisionsFromMakeDecisionResponse(MakeDecisionsAction md)
        {
            md.RemoveDecisions((IDecision d) => IsDay(d.SelectedCard));
            yield return DoNothing();
        }
        public override bool AskIfActionCanBePerformed(GameAction ga)
        {
            var nonVillainAffectingDay = ga.DoesFirstCardAffectSecondCard((Card c) => !c.IsVillain, (Card c) => IsDay(c));
            return nonVillainAffectingDay != true;
        }
        public override bool? AskIfCardIsVisibleToCardSource(Card c, CardSource source)
        {
            if(IsDay(c))
            {
                return source.Card.IsVillain;
            }
            return true;
        }

        private int NumFaceUpDays()
        {
            return GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsDay(c)).Count();
        }

        private string BuildNumberOfDaySpecialString()
        {
            int numFaceUpDays = NumFaceUpDays();
            string special = "";
            switch(numFaceUpDays)
            {
                case 0:
                    special = "Day 0: The timeline has yet to begin...";
                    break;
                case 1:
                    special = "Dawn of Day 1: 96 hours remain...";
                    break;
                case 2:
                    special = "Dawn of Day 2: 72 hours remain...";
                    break;
                case 3:
                    special = "Dawn of Day 3: 48 hours remain...";
                    break;
                case 4:
                    special = "Dawn of Day 4: 24 hours remain...";
                    break;
                default:
                    special = "Dawn of Day... wait how did you break out of the loop?";
                    break;
            }
            return special;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText($"Do you want to flip {CharacterCard.Title} and reset the timeline?", $"Should they flip {CharacterCard.Title} and reset the timeline?", $"Vote for flipping {CharacterCard.Title} and resetting the timeline?", $"flipping {CharacterCard.Title} and resetting the timeline");

        }
    }
}
