using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class TheRamUtilityCharacterCardController : VillainCharacterCardController
    {
        public TheRamUtilityCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        protected bool IsUpClose(Card c)
        {
            return c.IsTarget && IsUpClose(c.Owner);
        }

        protected bool IsUpClose(TurnTaker tt)
        {
            return tt.GetCardsWhere(HasUpCloseNextToCard).Count() > 0;
        }

        private bool HasUpCloseNextToCard(Card c)
        {
            if (c != null)
            {
                return c.NextToLocation.Cards.Where((Card nextTo) => nextTo.Identifier == "UpClose").Count() > 0;
            }
            return false;
        }

        protected IEnumerator PlayGivenUpCloseByGivenCard(Card upClose, Card target, bool isPutIntoPlay = false)
        {
            CardController cardController = FindCardController(upClose);
            if (cardController is UpCloseCardController upCloseController)
            {
                IEnumerator play = upCloseController.PlayBySpecifiedHero(target, isPutIntoPlay, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(play);
                }
                else
                {
                    GameController.ExhaustCoroutine(play);
                }
            }
            yield break;
        }

        protected void AddUpCloseTrackers()
        {
            SpecialStringMaker.ShowSpecialString(BuildUpCloseSpecialString);
            SpecialStringMaker.ShowSpecialString(BuildNotUpCloseSpecialString);
        }

        private string BuildUpCloseSpecialString()
        {
            var upCloseHeroes = FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && IsUpClose(tt)).ToList();
            string upCloseSpecial = "Heroes who are Up Close: ";
            if (upCloseHeroes.Any())
            {
                upCloseSpecial += upCloseHeroes.FirstOrDefault().NameRespectingVariant;
                for (int i = 1; i < upCloseHeroes.Count(); i++)
                {
                    upCloseSpecial += ", " + upCloseHeroes[i].NameRespectingVariant;
                }
            }
            else
            {
                upCloseSpecial += "None";
            }
            return upCloseSpecial;
        }

        private string BuildNotUpCloseSpecialString()
        {
            var notUpCloseHeroes = FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && !IsUpClose(tt)).ToList();
            string notUpCloseSpecial = "Heroes who are not Up Close: ";
            if (notUpCloseHeroes.Any())
            {
                notUpCloseSpecial += notUpCloseHeroes.FirstOrDefault().NameRespectingVariant;
                for (int i = 1; i < notUpCloseHeroes.Count(); i++)
                {
                    notUpCloseSpecial += ", " + notUpCloseHeroes[i].NameRespectingVariant;
                }
            }
            else
            {
                notUpCloseSpecial += "None";
            }
            return notUpCloseSpecial;
        }
    }
}
