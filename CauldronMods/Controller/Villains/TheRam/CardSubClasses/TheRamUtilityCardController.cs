using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class TheRamUtilityCardController : CardController
    {
        public TheRamUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected Card GetRam
        {
            get
            {
                if(TurnTaker.HasMultipleCharacterCards)
                {
                    Card ram = TurnTaker.CharacterCards.Where((Card c) => c.Identifier == "TheRamCharacter").FirstOrDefault();
                    if (Game.IsChallenge && ram != null && !ram.IsInPlay)
                    {
                        //If {TheRam} isn’t in play, other villain cards treat {AdmiralWintersCharacter} as {TheRam}.
                        return TurnTaker.CharacterCards.Where((Card c) => c.Identifier == "AdmiralWintersCharacter").FirstOrDefault();
                    }
                    return ram;

                }
                else
                {
                    return CharacterCard;
                }
            }
        }

        protected Card RamIfInPlay
        {
            get
            {
                Card ram = TurnTaker.CharacterCards.Where((Card c) => c.Identifier == "TheRamCharacter").FirstOrDefault();
                if (ram != null && ram.IsInPlay)
                {
                    return ram;
                }
                if(Game.IsChallenge && (ram == null || !ram.IsInPlay))
                {
                    var winters = TurnTaker.CharacterCards.Where((Card c) => c.Identifier == "AdmiralWintersCharacter").FirstOrDefault();
                    if (winters != null && winters.IsInPlay)
                    {
                        return winters;
                    }
                }
                return null;
            }
        }
        protected IEnumerator MessageNoRamToAct(CardSource actingCard, string missingAction = "act")
        {
            IEnumerator coroutine = GameController.SendMessageAction($"The Ram is not in play, so it does not {missingAction}.", Priority.Medium, actingCard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
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
            CardController upCloseController = FindCardController(upClose);
            if (upCloseController is UpCloseCardController)
            {
                IEnumerator play = (upCloseController as UpCloseCardController).PlayBySpecifiedHero(target, isPutIntoPlay, GetCardSource());
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

        protected bool IsDeviceOrNode(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "device") || GameController.DoesCardContainKeyword(c, "node");
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