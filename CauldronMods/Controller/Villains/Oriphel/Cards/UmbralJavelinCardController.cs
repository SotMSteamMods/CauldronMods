using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class UmbralJavelinCardController : OriphelUtilityCardController
    {
        public UmbralJavelinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => base.CharacterCard.Title == "Oriphel", () => "Oriphel is in play.", () => "Jade is in play.");

        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //"If Jade is in play, reveal the top {H} cards of the villain deck. Put any revealed Goons and Guardians into play and discard the rest.",
            if (jadeIfInPlay != null)
            {
                coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, H, new LinqCardCriteria((Card c) => IsGoon(c) || IsGuardian(c), "goon or guardian"));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //"If {Oriphel} is in play, destroy {H} hero ongoing and/or equipment cards. Play the top card of the villain deck."
            if (oriphelIfInPlay != null)
            {
                coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), H, false, H, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = PlayTheTopCardOfTheVillainDeckResponse(FakeAction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}