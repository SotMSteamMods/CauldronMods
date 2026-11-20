using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

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
/// These extensions provide a shared set of methods that can be used by
/// <see cref="DriftBaseCardController"/>, <see cref="DriftSubCharacterCardController"/>
/// and <see cref="DriftTurnTakerController"/>.
/// </summary>
public static class DriftShiftingExtensions
{
    extension(IDriftShifting shifting)
    {
        public string HasShiftedPropertyKey => "HasShifted";
        public string ShiftPoolIdentifier => "ShiftPool";
        public string ShiftTrackIdentifier => "ShiftTrack";
        public string Past => "{Past}";
        public string Future => "{Future}";
        public string BaseShiftTrackPrefix => "Base";
        public string DualShiftTrackPrefix => "Dual";
        public string ThroughTheBreachShiftTrackPrefix => "ThroughTheBreach";

        /// <summary>
        /// Returns the Shift Track prefix for the current variant
        /// </summary>
        /// <returns>The Shift Track prefix for the current variant</returns>
        public string ShiftTrackPrefix => shifting.ShiftingCharacterCardController switch
        {
            DualDriftCharacterCardController or FutureDriftCharacterCardController or PastDriftCharacterCardController => shifting.DualShiftTrackPrefix,
            ThroughTheBreachDriftCharacterCardController => shifting.ThroughTheBreachShiftTrackPrefix,
            _ => shifting.BaseShiftTrackPrefix
        };

        public string ShiftTrackIdentifierWithPrefix => shifting.ShiftTrackPrefix + shifting.ShiftTrackIdentifier;
        public string ShiftTrackIdentifierWithPrefixAndCurrentPosition => shifting.ShiftTrackIdentifierWithPrefix + shifting.CurrentShiftPosition;

        /// <summary>
        /// The current position on the Shift Track
        /// 
        /// If the Shift Track is not found, returns 0.
        /// </summary>
        /// <returns>The current shift position</returns>
        public int CurrentShiftPosition => shifting.CurrentShiftPool?.CurrentValue ?? 0;

        /// <summary>
        /// Returns the "ShiftPool" TokenPool for the current ShiftTrack
        /// </summary>
        /// <returns>The "ShiftPool" TokenPool associated with the current ShiftTrack</returns>
        public TokenPool CurrentShiftPool => shifting.CurrentShiftTrack?.FindTokenPool(shifting.ShiftPoolIdentifier);

        /// <summary>
        /// Returns the current ShiftTrack
        /// </summary>
        /// <returns>The current ShiftTrack</returns>
        public Card CurrentShiftTrack
        {
            get
            {
                if (shifting.ShiftingGameController is null) return null;

                return shifting.ShiftingGameController.FindCardsWhere(
                    (Card c) =>
                        c.SharedIdentifier == shifting.ShiftTrackIdentifier &&
                        c.IsInPlayAndHasGameText,
                    realCardsOnly: false
                ).FirstOrDefault();
            }
        }

        public CardController CurrentShiftTrackCardController => shifting.ShiftingGameController.FindCardController(shifting.CurrentShiftTrack);

        /// <summary>
        /// Verifies if the provided position is a valid position on the Shift Track.
        /// </summary>
        /// <param name="position">The position to validate. Valid range is 1 to 4 (inclusive)</param>
        /// <returns><c>true</c> if the position is on the Shift Track; otherwise, <c>false</c>.</returns>
        public bool IsValidShiftPosition(int position)
        {
            return 1 <= position && position <= 4;
        }



        /// <summary>
        /// Returns the Shift Track associated with <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position to get the Shift Track for</param>
        /// <returns>The Shift Track associated with <paramref name="position"/></returns>
        public Card GetPositionalShiftTrack(int position)
        {
            string fullShiftTrackIdentifier = shifting.ShiftTrackIdentifierWithPrefix + position;
            return shifting.ShiftingGameController.FindCardsWhere(
                        (Card c) => c.Identifier == fullShiftTrackIdentifier,
                        realCardsOnly: false
                    ).FirstOrDefault();
        }

        /// <summary>
        /// Returns whether the current Shift Track position is in the Past.
        /// 
        /// This is a wrapper around <see cref="DriftShiftingExtensions.IsTimeMatching(IDriftShifting, string)"/>
        /// Use this to avoid having to write <c>this.IsTimeMatching(this.Past)</c>
        /// </summary>
        /// <returns><c>true</c> if the current Shift Track position is in the Past; otherwise, <c>false</c>.</returns>
        public bool IsTimeMatchingPast => IsTimeMatching(shifting, shifting.Past);

        /// <summary>
        /// Returns whether the current Shift Track position is in the Future.
        /// 
        /// This is a wrapper around <see cref="DriftShiftingExtensions.IsTimeMatching(IDriftShifting, string)"/>
        /// Use this to avoid having to write <c>this.IsTimeMatching(this.Future)</c>
        /// </summary>
        /// <returns><c>true</c> if the current Shift Track position is in the Future; otherwise, <c>false</c>.</returns>
        public bool IsTimeMatchingFuture => IsTimeMatching(shifting, shifting.Future);

        /// <summary>
        /// Returns whether the current Shift Track position matches <paramref name="time"/>.
        /// 
        /// Do not use this function directly.
        /// Instead use <see cref="DriftShiftingExtensions.IsTimeMatchingFuture(IDriftShifting)"/> 
        /// or <see cref="DriftShiftingExtensions.IsTimeMatchingPast(IDriftShifting)"/>.
        /// </summary>
        /// <param name="time">The time to match against. Valid options are[{Past}|{Future}] </param>
        /// <returns><c>true</c> if the current Shift Track position matches <paramref name="time"/>; otherwise, <c>false</c>.</returns>
        private bool IsTimeMatching(string time) => shifting.CurrentShiftPosition switch
        {
            1 or 2 => time == shifting.Past,
            3 or 4 => time == shifting.Future,
            _ => false
        };

        /// <summary>
        /// Switches the Shift Track in use to the one offset by <paramref name="newPositionModifier"/>
        /// 
        /// The potential new position will be validated before any changes are made, and no changes
        /// will be made if the new position is invalid.
        /// </summary>
        /// <param name="newPositionModifier">The offset to change the Shift Track position by</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator SwitchTrack(int newPositionModifier = 0)
        {
            if (newPositionModifier == 0) yield break;
            int newShiftPosition = shifting.CurrentShiftPosition + newPositionModifier;
            if (!shifting.IsValidShiftPosition(newShiftPosition)) yield break;

            string newTrackIdentifier = shifting.ShiftTrackIdentifierWithPrefix + newShiftPosition;
            IEnumerator coroutine = shifting.ShiftingGameController.SwitchCards(
                    cardA: shifting.CurrentShiftTrack,
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
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftL(List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 1;
            numberOfShifts ??= [];

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
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftLL(List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 2;
            numberOfShifts ??= [];

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
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftLLL(List<bool> numberOfShifts = null)
        {
            int direction = -1;
            int amount = 3;
            numberOfShifts ??= [];

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
        public IEnumerator ShiftR(List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 1;
            numberOfShifts ??= [];

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
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftRR(List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 2;
            numberOfShifts ??= [];

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
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftRRR(List<bool> numberOfShifts = null)
        {
            int direction = 1;
            int amount = 3;
            numberOfShifts ??= [];

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
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="shiftText">The shift icon to display in DualDrift's prompts</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        private IEnumerator PerformShiftAction(int direction, int amount, string shiftText, List<bool> numberOfShifts)
        {
            IEnumerator shiftingCoroutine = shifting.ShiftAction(direction, amount, numberOfShifts);
            if (shifting.ShiftingGameController.FindCardController(shifting.CurrentShiftTrack) is DualShiftTrackUtilityCardController dualShiftTrack &&
                !dualShiftTrack.HasTrackAbilityBeenActivated)
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
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        public IEnumerator ShiftAction(int direction, int amount, List<bool> numberOfShifts)
        {

            Func<TokenPool, int, CardSource, IEnumerator> tokenPoolManipulationFunc = direction switch
            {
                < 0 => (TokenPool pool, int amount, CardSource source) =>
                        shifting.ShiftingGameController.RemoveTokensFromPool(pool, amount, cardSource: source),
                > 0 => shifting.ShiftingGameController.AddTokensToPool,
                0   => (TokenPool pool, int amount, CardSource source) =>
                         shifting.ShiftingGameController.DoNothing()
            };

            IEnumerator coroutine;
            for (int i = 0; i < amount; i++)
            {
                if (!shifting.IsValidShiftPosition(shifting.CurrentShiftPosition + direction))
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
                        key: shifting.HasShiftedPropertyKey,
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

                coroutine = tokenPoolManipulationFunc(shifting.CurrentShiftPool, 1, shifting.ShiftingCardSource);
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
        /// <param name="dualShiftTrack">The current Shift Track for Dual Drift</param>
        /// <param name="direction">Whether to shift right or left. Any positive value will shift right. Any negative value will shift left.</param>
        /// <param name="amount">How far to shift in the specified direction</param>
        /// <param name="shiftText">The shift icon to display in DualDrift's prompts</param>
        /// <param name="numberOfShifts">The List that stores the number of shifts performed. Use <c>numberOfShifts.Count()</c> to retreive the number of shifts performed.</param>
        /// <returns>The coroutine to be executed.</returns>
        private IEnumerator DualDriftShifting(DualShiftTrackUtilityCardController dualShiftTrack, int direction, int amount, string shiftText, List<bool> numberOfShifts)
        {
            string option1 = "Shift " + shiftText;
            string option2 = "Swap the active Drift, then Shift " + shiftText;
            string option3 = "Shift " + shiftText + ", then swap the active Drift";
            string[] options = [
                option1, option2, option3];

            List<SelectWordDecision> storedResults = [];
            SelectWordDecision decision = new(
                shifting.ShiftingGameController,
                shifting.ShiftingDecisionMaker,
                SelectionType.Custom,
                options,
                cardSource: shifting.ShiftingCardSource,
                associatedCards: shifting.CurrentShiftTrack.ToEnumerable())
            {
                ExtraInfo = () =>
                    $"{dualShiftTrack.GetInactiveCharacterCard().Title} is at position {dualShiftTrack.InactiveCharacterPosition()}"
            };
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

            string selectedWord = storedResults.FirstOrDefault((SelectWordDecision d) =>
                    d.Completed && d.SelectedWord != null)?.SelectedWord;

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
        /// <param name="decision">The decision that has SelectionType.Custom and the UI wants text for.</param>
        /// <returns>A populated CustomDecisionText object for UI display for Dual Drift shifting.</returns>
        public CustomDecisionText GetDualDriftCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText($"What do you want to do?", $"What should they do?", $"Vote for what they should do.", $"what to do");
        }
    }
}
