using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cauldron.ScreaMachine
{
    public static class ScreaMachineBandmate
    {
        public enum Value
        {
            Unknown = 0,
            Slice,
            Bloodlace,
            Valentine,
            RickyG
        }

        private static readonly Dictionary<Value, string> _Keywords = new Dictionary<Value, string>()
        {
            [Value.Slice] = "guitarist",
            [Value.Bloodlace] = "bassist",
            [Value.Valentine] = "vocalist",
            [Value.RickyG] = "drummer",
        };

        public static string GetKeyword(this Value member)
        {
            return _Keywords[member];
        }

        public static IEnumerable<string> Keywords => _Keywords.Values;

        private static readonly Dictionary<Value, string> _AbilityKeys = new Dictionary<Value, string>()
        {
            [Value.Slice] = "{Guitar}",
            [Value.Bloodlace] = "{Bass}",
            [Value.Valentine] = "{Vocal}",
            [Value.RickyG] = "{Drum}",
        };

        public static string GetAbilityKey(this Value member)
        {
            return _AbilityKeys[member];
        }

        public static IEnumerable<string> AbilityKeys => _AbilityKeys.Values;

        private static readonly Dictionary<Value, string> _Identifiers = new Dictionary<Value, string>()
        {
            [Value.Slice] = "SilceCharacter",
            [Value.Bloodlace] = "BloodlaceCharacter",
            [Value.Valentine] = "ValentineCharacter",
            [Value.RickyG] = "RickyGCharacter",
        };

        public static string GetIdentifier(this Value member)
        {
            return _Identifiers[member];
        }

        public static IEnumerable<string> Identifiers => _Identifiers.Values;
    }
}
