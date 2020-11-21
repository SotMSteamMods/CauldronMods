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
        public UpCloseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            if (!Card.Location.IsNextToCard && AllHeroesAreUpClose)
            {
                IEnumerator message = GameController.SendMessageAction("All heroes were already Up Close, so the new one destroys itself.", Priority.High, GetCardSource());
                IEnumerator destroy = DestroyThisCardResponse(FakeAction);
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
            //"Play this card next to a hero who is not Up Close."
            Location targetLocation = overridePlayArea;
            bool entersPlayNextToHero = targetLocation != null && targetLocation.IsNextToCard && targetLocation.OwnerCard.IsHeroCharacterCard && !IsUpClose(targetLocation.OwnerCard);
            IEnumerator coroutine;
            if (entersPlayNextToHero || AllHeroesAreUpClose)
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
                    new Power(heroController.HeroTurnTakerController, heroController, "Destroy Up Close.", DestroyThisCardResponse(FakeAction), 0, null, GetCardSource())
                };
            }
            return null;
        }

        public IEnumerator PlayBySpecifiedHero(Card hero, CardSource cardSource)
        {
            Location targetLocation = null;
            if (hero != null && hero.Owner.IsHero && hero.IsHeroCharacterCard && !hero.Owner.IsIncapacitatedOrOutOfGame && !IsUpClose(hero))
            {
                targetLocation = hero.NextToLocation;
            }
            else
            {
                targetLocation = null;
            }
            IEnumerator play = GameController.PlayCard(TurnTakerController, this.Card, wasCardPlayed: new List<bool> { true }, overridePlayLocation: targetLocation, cardSource: cardSource);
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
                return FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame && !IsUpClose(tt)).Count() == 0;

            }
        }
        private Func<Card, bool> NotUpCloseCriteria
        {
            get
            {
                return (Card c) => c.IsInPlayAndHasGameText &&
                                       c.IsHeroCharacterCard &&
                                       c.Owner.IsHero &&
                                       !c.Owner.ToHero().IsIncapacitatedOrOutOfGame &&
                                       !IsUpClose(c.Owner);
            }
        }

        private GameAction FakeAction
        {
            get
            {
                return new CardEntersPlayAction(GetCardSource(), Card, true, TurnTakerController, this.Card.Location);
            }
        }
    }
}