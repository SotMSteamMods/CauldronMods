using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Handelabra;

namespace Cauldron
{
    public static class ExtensionMethods
    {
        public static void ReorderTokenPool(this TokenPool[] tokenPools, string poolThatShouldBeFirst)
        {
            var temp = new List<TokenPool>(tokenPools);
            int targetIndex = temp.FindIndex(tp => string.Equals(tp.Identifier, poolThatShouldBeFirst, StringComparison.Ordinal));
            //if targetIndex == -1, no matching pool found, make no change.
            //if targetIndex == 0, matching pool already first, make no change.
            if (targetIndex > 0)
            {
                var newFirst = tokenPools[targetIndex];

                //shuffle all other indexes forward without changing the relative order
                int index = targetIndex;
                while (index > 0)
                {
                    tokenPools[index] = tokenPools[--index];
                }
                tokenPools[0] = newFirst;
            }
        }

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

        public static SpecialString ShowHeroWithMostCardsInTrash(this SpecialStringMaker maker, LinqCardCriteria additionalCriteria = null, Func<bool> showInEffectsList = null)
        {
            CardController _cardController;
            var p1 = maker.GetType().GetField(nameof(_cardController), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            _cardController = (CardController)p1.GetValue(maker);

            return maker.ShowSpecialString(delegate
            {
                IEnumerable<TurnTaker> enumerable = _cardController.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, _cardController.BattleZone);
                List<string> list = new List<string>();
                int num = 0;
                foreach (HeroTurnTaker hero in enumerable)
                {
                    IEnumerable<Card> cardsWhere = hero.GetCardsWhere((Card c) => c.IsInTrash && c.Location.OwnerTurnTaker == hero);
                    List<Card> source = ((additionalCriteria == null) ? cardsWhere.ToList() : cardsWhere.Where(additionalCriteria.Criteria).ToList());
                    if (source.Count() > num)
                    {
                        list.RemoveAll((string htt) => true);
                        list.Add(hero.Name);
                        num = source.Count();
                    }
                    else if (source.Count() == num)
                    {
                        list.Add(hero.Name);
                    }
                }
                string text = list.Count().ToString_SingularOrPlural("Hero", "Heroes");
                string text2 = " in trash";
                string text3 = " cards";
                if (additionalCriteria != null)
                {
                    text3 = " " + additionalCriteria.GetDescription();
                }
                return (list.Count() > 0) ? string.Format("{0} with the most{3}{2}: {1}.", text, list.ToRecursiveString(), text2, text3) : "Warning: No heroes found";
            }, showInEffectsList);
        }
                    Log.Debug(text + " has already been unlocked!");
                Log.Debug(text + " has not already been unlocked!");
    }
}
