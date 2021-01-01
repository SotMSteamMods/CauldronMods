using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cauldron
{
    public static class ExtensionMethods
    {
        public static void IncrementCardProperty(this CardController card, string key, int adjust = 1)
        {
            int num = card.GetCardPropertyJournalEntryInteger(key) ?? 0;
            card.SetCardProperty(key, num + adjust);
        }

        public static Card GetCardDestroyer(this DestroyCardAction action)
        {
            if (action.ResponsibleCard != null)
                return action.ResponsibleCard;
            if (action.CardSource != null)
                return action.CardSource.Card;

            return null;
        }

        public static bool WasDestroyedBy(this DestroyCardAction action, Func<Card, bool> criteria)
        {
            var card = GetCardDestroyer(action);
            return criteria(card);
        }

        public static void SetPowerNumeralsArray(this ReflectionStatusEffect effect, int[] array)
        {
            var p1 = effect.GetType().GetProperty(nameof(effect.PowerNumeralsToChange));
            var p2 = p1.DeclaringType.GetProperty(nameof(effect.PowerNumeralsToChange));

            p2.SetValue(effect, array, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);
        }

    }
}
