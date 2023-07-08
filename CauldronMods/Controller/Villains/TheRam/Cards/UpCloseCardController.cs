using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace Cauldron.TheRam
{
    public class UpCloseCardController : TheRamUtilityCardController
    {
        private Location NextToHeroToGoTo = null;
        public UpCloseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            if (!Card.Location.IsNextToCard && AllHeroesAreUpClose)
            {
                IEnumerator message = GameController.SendMessageAction("All heroes were already Up Close, so the new one destroys itself.", Priority.High, GetCardSource());
                IEnumerator destroy = DestroyThisCardResponse(null);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                    yield return GameController.StartCoroutine(destroy);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                    GameController.ExhaustCoroutine(destroy);
                }
            }
            yield break;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //if we're given a hero to go to, go there - if we can
            if (NextToHeroToGoTo != null)
            {
                if (NextToHeroToGoTo.IsNextToCard && IsHeroCharacterCard(NextToHeroToGoTo.OwnerCard) && !IsUpClose(NextToHeroToGoTo.OwnerCard))
                {
                    storedResults.Add(new MoveCardDestination(NextToHeroToGoTo));
                    NextToHeroToGoTo = null;
                    yield break;
                }
                NextToHeroToGoTo = null;
            }

            //"Play this card next to a hero who is not Up Close."
            IEnumerator coroutine;
            if (AllHeroesAreUpClose)
            {
                coroutine = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            }
            else
            {
                coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(NotUpCloseCriteria, "active heroes who are not Up Close", useCardsSuffix: false), storedResults, isPutIntoPlay, decisionSources);
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"[The hero this is next to] gains:",
            //"Power: Destroy this card."
            AddAsPowerContributor();
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController heroController)
        {
            if (heroController.Card == GetCardThisCardIsNextTo())
            {
                return new Power[1]
                {
                    new Power(heroController.HeroTurnTakerController, heroController, "Destroy Up Close.", DestroyThisCardResponse(null), 0, null, GetCardSource())
                };
            }
            return null;
        }

        public IEnumerator PlayBySpecifiedHero(Card hero, bool isPutIntoPlay, CardSource cardSource)
        {
            if (hero != null && IsHero(hero.Owner) && IsHeroCharacterCard(hero) && !hero.Owner.IsIncapacitatedOrOutOfGame && !IsUpClose(hero))
            {
                NextToHeroToGoTo = hero.NextToLocation;
            }

            IEnumerator play = GameController.PlayCard(TurnTakerController, this.Card, wasCardPlayed: new List<bool>() { !isPutIntoPlay }, isPutIntoPlay: isPutIntoPlay, cardSource: cardSource);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(play);
            }
            else
            {
                GameController.ExhaustCoroutine(play);
            }
            yield break;
        }

        private bool AllHeroesAreUpClose
        {
            get
            {
                return FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && !tt.ToHero().IsIncapacitatedOrOutOfGame && !IsUpClose(tt)).Count() == 0;

            }
        }
        private Func<Card, bool> NotUpCloseCriteria
        {
            get
            {
                return (Card c) => c.IsInPlayAndHasGameText &&
                                        IsHeroCharacterCard(c) &&
                                       IsHero(c.Owner) &&
                                       !c.Owner.ToHero().IsIncapacitatedOrOutOfGame &&
                                       !IsUpClose(c.Owner);
            }
        }
    }
}