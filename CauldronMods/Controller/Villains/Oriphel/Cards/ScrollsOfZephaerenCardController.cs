using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class ScrollsOfZephaerenCardController : OriphelUtilityCardController
    {
        public ScrollsOfZephaerenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //"Shuffle all Guardians from the villain trash into the villain deck.",
            IEnumerator coroutine = GameController.MoveCards(TurnTakerController,
                                                        FindCardsWhere((Card c) => c.Location == TurnTaker.Trash && IsGuardian(c)),
                                                        TurnTaker.Deck,
                                                        cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"Play the top card of the villain deck.",
            coroutine = PlayTheTopCardOfTheVillainDeckResponse(FakeAction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"If {Oriphel} is in play, he deals the hero target with the second lowest HP 3 melee damage."
            if (oriphelIfInPlay != null)
            {
                coroutine = DealDamageToLowestHP(oriphelIfInPlay, 2, (Card c) => IsHero(c), (c) => 3, DamageType.Melee);
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