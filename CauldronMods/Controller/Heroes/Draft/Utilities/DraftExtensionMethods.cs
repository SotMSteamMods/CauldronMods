using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Draft
{
    public static class DraftExtensionMethods
    {
        private const string ShiftTrackPositionKey = "ShiftTrackPosition";
        public static void SetShiftTrackPosition(this CardController _cardController, int position)
        {
            position = position < 1 ? 1 : position > 4 ? 4 : position; // basic clamping to position restrictions

            _cardController.SetCardProperty(ShiftTrackPositionKey, position);
        }

        public static void ShiftL(this CardController _cardController)
        {
            int? currentPosition = _cardController.GetCardPropertyJournalEntryInteger(ShiftTrackPositionKey);
            if (currentPosition is null)
            {
                Log.Debug("Couldn't shift because position is not currently set!");
                return;
            }
            int newPosition = currentPosition.Value - 1;
            _cardController.SetShiftTrackPosition(newPosition);
        }

        public static void ShiftR(this CardController _cardController)
        {
            int? currentPosition = _cardController.GetCardPropertyJournalEntryInteger(ShiftTrackPositionKey);
            if (currentPosition is null)
            {
                Log.Debug("Couldn't shift because position is not currently set!");
                return;
            }
            int newPosition = currentPosition.Value + 1;
            _cardController.SetShiftTrackPosition(newPosition);
        }

        public static void ShiftRR(this CardController _cardController)
        {
            _cardController.ShiftR();
            _cardController.ShiftR();
        }

        public static void ShiftLL(this CardController _cardController)
        {
            _cardController.ShiftL();
            _cardController.ShiftL();
        }

        public static void ShiftRRR(this CardController _cardController)
        {
            _cardController.ShiftR();
            _cardController.ShiftR();
            _cardController.ShiftR();
        }

        public static void ShiftLLL(this CardController _cardController)
        {
            _cardController.ShiftL();
            _cardController.ShiftL();
            _cardController.ShiftL();
        }
    }
}
