using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    /// <summary>
    /// An interface that attaches the collection of shifting methods
    /// from <see cref="DriftShiftingExtensions"/> to the attached class.
    /// </summary>
    public interface IDriftShifting
    {
        /// <summary>
        /// The CustomText mode to use for a given CustomTextDecision.
        /// 
        /// Since there are four different CustomTextDecisions throughout
        /// Drift's code, this enum is used to differentiate between them.
        /// Set <see cref="CurrentCustomTextMode"/> to the appropriate value
        /// immediately before calling a Decision with <see cref="SelectionType.Custom"/>
        /// 
        /// To aid readability, add <c>using static Cauldron.Drift.IDriftShifting</c>
        /// at the top of any class that uses this enum.
        /// </summary>
        public enum DriftCustomTextMode
        {
            AskToShift,
            DualDriftShift,
            StartOfGameChooseDrift,
            StartOfGameChooseShiftTrackPosition
        }

        public DriftCustomTextMode CurrentCustomTextMode { get; set; }
        public GameController ShiftingGameController { get; }
        public CardSource ShiftingCardSource { get; }
        public HeroTurnTakerController ShiftingDecisionMaker { get; }
        public CharacterCardController ShiftingCharacterCardController { get; }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="IDriftShifting"/> interface.
    /// 
    /// These extensions provide a shared set of methods that can be used by both
    /// <see cref="DriftBaseCardController"/> and <see cref="DriftSubCharacterCardController"/>
    /// and <see cref="DriftTurnTakerController"/>.
    /// </summary>
    public static class DriftShiftingExtensions
    {
        public static string HasShiftedPropertyKey(this IDriftShifting shifting) => "HasShifted";
        public static string ShiftPoolIdentifier(this IDriftShifting shifting) => "ShiftPool";
        public static string ShiftTrackIdentifier(this IDriftShifting shifting) => "ShiftTrack";
        public static string Past(this IDriftShifting shifting) => "{Past}";
        public static string Future(this IDriftShifting shifting) => "{Future}";
        public static string BaseShiftTrackPrefix(this IDriftShifting shifting) => "Base";
        public static string DualShiftTrackPrefix(this IDriftShifting shifting) => "Dual";
        public static string ThroughTheBreachShiftTrackPrefix(this IDriftShifting shifting) => "ThroughTheBreach";
        public static string ShiftTrackIdentifierWithPrefix(this IDriftShifting shifting) => shifting.ShiftTrackPrefix() + shifting.ShiftTrackIdentifier();
        public static string ShiftTrackIdentifierWithPrefixAndCurrentPosition(this IDriftShifting shifting) => shifting.ShiftTrackIdentifierWithPrefix() + shifting.CurrentShiftPosition();

        /// <summary>
        /// Returns the current position on the Shift Track
        /// 
        /// If the Shift Track is not found, returns 0.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns>The current shift position</returns>
        public static int CurrentShiftPosition(this IDriftShifting shifting)
        {
            return shifting.GetShiftPool()?.CurrentValue ?? 0;
        }

        /// <summary>
        /// Verifies if the provided position is a valid position on the Shift Track.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="position">The position to validate. Valid range is 1 to 4 (inclusive)</param>
        /// <returns><c>true</c> if the position is on the Shift Track; otherwise, <c>false</c>.</returns>
        public static bool IsValidShiftPosition(this IDriftShifting shifting, int position)
        {
            return 1 <= position && position <= 4;
        }

        /// <summary>
        /// Returns the "ShiftPool" TokenPool for the current ShiftTrack
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns>The "ShiftPool" TokenPool associated with the current ShiftTrack</returns>
        public static TokenPool GetShiftPool(this IDriftShifting shifting)
        {
            return shifting.GetShiftTrack()?.FindTokenPool(ShiftPoolIdentifier(shifting));
        }

        /// <summary>
        /// Returns the current ShiftTrack
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns>The current ShiftTrack</returns>
        public static Card GetShiftTrack(this IDriftShifting shifting)
        {
            if (shifting.ShiftingGameController is null) return null;

            return shifting.ShiftingGameController.FindCardsWhere(
                (Card c) =>
                    c.SharedIdentifier == ShiftTrackIdentifier(shifting) && 
                    c.IsInPlayAndHasGameText, 
                realCardsOnly: false
            ).FirstOrDefault();
        }

        /// <summary>
        /// Returns the Shift Track prefix for the current variant
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns>The Shift Track prefix for the current variant</returns>
        public static string ShiftTrackPrefix(this IDriftShifting shifting)
        {
            string shiftTrackPrefix = shifting.BaseShiftTrackPrefix();
            if (shifting.ShiftingCharacterCardController is DualDriftSubCharacterCardController)
            {
                shiftTrackPrefix = shifting.DualShiftTrackPrefix();
            }
            else if (shifting.ShiftingCharacterCardController is ThroughTheBreachSubCharacterCardController)
            {
                shiftTrackPrefix = shifting.ThroughTheBreachShiftTrackPrefix();
            }
            return shiftTrackPrefix;
        }

        /// <summary>
        /// Returns the Shift Track associated with <paramref name="position"/>.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="position">The position to get the Shift Track for</param>
        /// <returns>The Shift Track associated with <paramref name="position"/></returns>
        public static Card GetPositionalShiftTrack(this IDriftShifting shifting, int position)
        {
            string fullShiftTrackIdentifier = shifting.ShiftTrackIdentifierWithPrefix() + position;
            return shifting.ShiftingGameController.FindCardsWhere(
                        (Card c) => c.Identifier == fullShiftTrackIdentifier, 
                        realCardsOnly: false
                    ).FirstOrDefault();
        }

        /// <summary>
        /// Returns whether the current Shift Track position is in the Past.
        /// 
        /// This is a wrapper around <see cref="DriftShiftingExtensions.IsTimeMatching(IDriftShifting, string)"/>
        /// Use this to avoid having to write <c>this.IsTimeMatching(this.Past())</c>
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns><c>true</c> if the current Shift Track position is in the Past; otherwise, <c>false</c>.</returns>
        public static bool IsTimeMatchingPast(this IDriftShifting shifting)
        {
            return IsTimeMatching(shifting, shifting.Past());
        }


        /// <summary>
        /// Returns whether the current Shift Track position is in the Future.
        /// 
        /// This is a wrapper around <see cref="DriftShiftingExtensions.IsTimeMatching(IDriftShifting, string)"/>
        /// Use this to avoid having to write <c>this.IsTimeMatching(this.Future())</c>
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <returns><c>true</c> if the current Shift Track position is in the Future; otherwise, <c>false</c>.</returns>
        public static bool IsTimeMatchingFuture(this IDriftShifting shifting)
        {
            return IsTimeMatching(shifting, shifting.Future());
        }

        /// <summary>
        /// Returns whether the current Shift Track position matches <paramref name="time"/>.
        /// 
        /// Do not use this function directly.
        /// Instead use <see cref="DriftShiftingExtensions.IsTimeMatchingFuture(IDriftShifting)"/> 
        /// or <see cref="DriftShiftingExtensions.IsTimeMatchingPast(IDriftShifting)"/>.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="time">The time to match against. Valid options are[{Past}|{Future}] </param>
        /// <returns><c>true</c> if the current Shift Track position matches <paramref name="time"/>; otherwise, <c>false</c>.</returns>
        private static bool IsTimeMatching(this IDriftShifting shifting, string time)
        {
            switch(shifting.CurrentShiftPosition())
            {
                case 1: case 2:
                    return time == shifting.Past();
                case 3: case 4:
                    return time == shifting.Future();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Switches the Shift Track in use to the one offset by <paramref name="newPositionModifier"/>
        /// 
        /// The potential new position will be validated before any changes are made, and no changes
        /// will be made if the new position is invalid.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="newPositionModifier">The offset to change the Shift Track position by</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator SwitchTrack(this IDriftShifting shifting, int newPositionModifier = 0)
        {
            if (newPositionModifier == 0) yield break;
            int newShiftPosition = shifting.CurrentShiftPosition() + newPositionModifier;
            if (!shifting.IsValidShiftPosition(newShiftPosition)) yield break;

            string newTrackIdentifier = shifting.ShiftTrackIdentifierWithPrefix() + newShiftPosition;
            IEnumerator coroutine = shifting.ShiftingGameController.SwitchCards(
                    cardA: shifting.GetShiftTrack(), 
                    cardB: shifting.ShiftingGameController.FindCardsWhere(
                        (Card c) => 
                            c.Identifier == newTrackIdentifier, 
                            realCardsOnly: false
                        ).FirstOrDefault(),
                    cardSource: shifting.ShiftingCardSource);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift left once.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftL(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 1;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftL}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift left twice.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftLL(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 2;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftLL}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift left thrice.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftLLL(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 3;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftLLL}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift right once.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftR(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 1;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftR}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift right twice.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftRR(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 2;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftRR}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Shifts Drift right thrice.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftRRR(this IDriftShifting shifting, List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 3;
            if (numberOfShifts is null) numberOfShifts = new List<bool>();

            IEnumerator coroutine = shifting.PerformShiftAction(direction, amount, "{ShiftRRR}", numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Performs a shift in the specified direction and amount.
        /// 
        /// This is a wrapper around <see cref="DriftShiftingExtensions.ShiftAction(IDriftShifting, int, int, List{bool})"/> that
        /// instead calls <see cref="DriftShiftingExtensions.DualDriftShifting(IDriftShifting, DualShiftTrackUtilityCardController, int, int, string, List{bool})"/>
        /// if the current variant is Dual Drift.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="shiftText">The shift icon to display in DualDrift's prompts</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        private static IEnumerator PerformShiftAction(this IDriftShifting shifting, int direction, int amount, string shiftText, List<bool> numberOfShifts)
        {
            IEnumerator shiftingCoroutine = shifting.ShiftAction(direction, amount, numberOfShifts);
            if (shifting.ShiftingGameController.FindCardController(shifting.GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack &&
                !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                shiftingCoroutine = shifting.DualDriftShifting(dualShiftTrack, direction, amount, shiftText, numberOfShifts);
            }

            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(shiftingCoroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(shiftingCoroutine);
            }
        }

        /// <summary>
        /// Performs a shift in the specified direction and amount.
        /// 
        /// A shift action consists of the following steps:
        /// <list type="number">
        /// <item><description>Updating <paramref name="numberOfShifts"/></description></item>
        /// <item><description>Updating the "HasShifted" property in the Journal to be <c>true</c></description></item>
        /// <item><description>Switching the Shift Track to the one matching the new position</description></item>
        /// <item><description>Updating the "ShiftPool" TokenPool count on the new Shift Track to match the new position</description></item>
        /// </list>
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public static IEnumerator ShiftAction(this IDriftShifting shifting, int direction, int amount, List<bool> numberOfShifts)
        {
            Func<TokenPool, int, CardSource, IEnumerator> tokenPoolManipulationFunc = shifting.ShiftingGameController.AddTokensToPool;
            if (direction < 1)
            {
                tokenPoolManipulationFunc = (TokenPool pool, int amount, CardSource source) =>
                    shifting.ShiftingGameController.RemoveTokensFromPool(pool, amount, cardSource: source);
            }

            IEnumerator coroutine;
            for (int i = 0; i < amount; i++)
            {
                if (!shifting.IsValidShiftPosition(shifting.CurrentShiftPosition() + direction))
                {
                    coroutine = shifting.ShiftingGameController.SendMessageAction("Drift has reached the end of the Shift Track", Priority.Medium, shifting.ShiftingCardSource);
                    if (shifting.ShiftingGameController.UseUnityCoroutines)
                    {
                        yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                    }

                    yield break;
                }

                numberOfShifts.Add(true);

                // Use logic from CardController.SetCardPropertyToTrueIfRealAction
                if (!shifting.ShiftingGameController.PretendMode &&
                    !shifting.ShiftingGameController.PreviewMode)
                {
                    shifting.ShiftingGameController.AddCardPropertyJournalEntry(
                        card: shifting.ShiftingCardSource.Card,
                        key: HasShiftedPropertyKey(shifting),
                        boolValue: true);
                }

                coroutine = shifting.SwitchTrack(direction);
                if (shifting.ShiftingGameController.UseUnityCoroutines)
                {
                    yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                }
                else
                {
                    shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                }

                coroutine = tokenPoolManipulationFunc(shifting.GetShiftPool(), 1, shifting.ShiftingCardSource);
                if (shifting.ShiftingGameController.UseUnityCoroutines)
                {
                    yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                }
                else
                {
                    shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        /// <summary>
        /// Provides prompts for swapping 1929 and 2199 Drifts while shifting
        /// the specified direction and amount.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="dualShiftTrack">The current Shift Track for Dual Drift</param>
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="shiftText">The shift icon to display in DualDrift's prompts</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        private static IEnumerator DualDriftShifting(this IDriftShifting shifting, DualShiftTrackUtilityCardController dualShiftTrack, int direction, int amount, string shiftText, List<bool> numberOfShifts)
        {
            string option1 = "Shift " + shiftText;
            string option2 = "Swap the active Drift, then Shift " + shiftText;
            string option3 = "Shift " + shiftText + ", then swap the active Drift";
            string[] options = [option1, option2, option3];

            List<SelectWordDecision> storedResults = new List<SelectWordDecision>();
            SelectWordDecision decision = new SelectWordDecision(
                shifting.ShiftingGameController, 
                shifting.ShiftingDecisionMaker, 
                SelectionType.Custom, 
                options, 
                cardSource: shifting.ShiftingCardSource, 
                associatedCards: shifting.GetShiftTrack().ToEnumerable());
            decision.ExtraInfo = () =>  
                $"{dualShiftTrack.GetInactiveCharacterCard().Title} is at position {dualShiftTrack.InactiveCharacterPosition()}";
            storedResults.Add(decision);
            shifting.CurrentCustomTextMode = IDriftShifting.DriftCustomTextMode.DualDriftShift;
            IEnumerator coroutine = shifting.ShiftingGameController.MakeDecisionAction(decision);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }

            string selectedWord = storedResults.Where((SelectWordDecision d) => 
                    d.Completed && d.SelectedWord != null)
                .FirstOrDefault()?.SelectedWord;

            if (selectedWord is null)
            {
                coroutine = shifting.ShiftAction(direction, amount, numberOfShifts);
                if (shifting.ShiftingGameController.UseUnityCoroutines)
                {
                    yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                }
                else
                {
                    shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }

            // Swap Before Shifting?
            if (selectedWord == option2)
            {
                coroutine = dualShiftTrack.SwapActiveDrift();
                if (shifting.ShiftingGameController.UseUnityCoroutines)
                {
                    yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                }
                else
                {
                    shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                }
            }

            // Shift
            coroutine = shifting.ShiftAction(direction, amount, numberOfShifts);
            if (shifting.ShiftingGameController.UseUnityCoroutines)
            {
                yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
            }
            else
            {
                shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
            }

            // Swap After Shifting?
            if (selectedWord == option3)
            {
                coroutine = dualShiftTrack.SwapActiveDrift();
                if (shifting.ShiftingGameController.UseUnityCoroutines)
                {
                    yield return shifting.ShiftingGameController.StartCoroutine(coroutine);
                }
                else
                {
                    shifting.ShiftingGameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        /// <summary>
        /// The CustomDecisionText for Dual Drift when swapping between 1929 nad 2199 Drifts.
        /// 
        /// Note that since this extension method can't actually override the function in this interface,
        /// this needs to be wrapped in an appropriate function that overrides
        /// <see cref="CardController.GetCustomDecisionText(IDecision)"/>.
        /// </summary>
        /// <param name="shifting">The instance of IDriftShifting being extended</param>
        /// <param name="decision">The decision that has SelectionType.Custom and the UI wants text for.</param>
        /// <returns>A populated CustomDecisionText object for UI display for Dual Drift shifting.</returns>
        public static CustomDecisionText GetDualDriftCustomDecisionText(this IDriftShifting shifting, IDecision decision)
        {
            return new CustomDecisionText($"What do you want to do?", $"What should they do?", $"Vote for what they should do.", $"what to do");
        }
    }
}
