using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class SandstormCardController : OriphelUtilityCardController
    {
        public SandstormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"));
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => IsGoon(c), "goon"));
        }

        public override IEnumerator Play()
        {
            //"Shuffle the villain trash and reveal cards until X Goons are revealed, where X is 1 plus the number of environment cards in play.",
            //"Put the revealed Goons into play and discard the other cards."
            //this function autoshuffles the trash, so we don't need to specify it ourselves
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Trash, null, new LinqCardCriteria((Card c) => IsGoon(c)), revealUntilNumberOfMatchingCards: EnoughGoons);
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

        private int EnoughGoons
        {
            get
            {
                return FindCardsWhere((Card c) => c.IsInPlay && c.IsEnvironment).Count() + 1;
            }
        }
    }
}