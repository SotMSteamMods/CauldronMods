using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Anathema
{
    // Token: 0x02001A15 RID: 6677
    public class AnathemaTurnTakerController : TurnTakerController
    {
        public AnathemaTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            if (base.CharacterCardController is AnathemaCharacterCardController || base.CharacterCardController is AcceleratedEvolutionAnathemaCharacterCardController)
            {
                IEnumerator arms = base.PutCardsIntoPlay(new LinqCardCriteria((Card c) => this.IsArm(c), "arm"), 2, false);
                IEnumerator body = base.PutCardsIntoPlay(new LinqCardCriteria((Card c) => this.IsBody(c), "body"), 1, false);
                IEnumerator head = base.PutCardsIntoPlay(new LinqCardCriteria((Card c) => this.IsHead(c), "head"), 1, true);


                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(arms);
                    yield return base.GameController.StartCoroutine(body);
                    yield return base.GameController.StartCoroutine(head);

                }
                else
                {
                    base.GameController.ExhaustCoroutine(arms);
                    base.GameController.ExhaustCoroutine(body);
                    base.GameController.ExhaustCoroutine(head);
                }

                if (base.GameController.Game.IsChallenge && base.CharacterCardController is AnathemaCharacterCardController)
                {
                    IEnumerator biofeedback = base.PutCardIntoPlay("Biofeedback");
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(biofeedback);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(biofeedback);
                    }
                }
            }
            yield break;
        }

        private bool IsArm(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "arm");
        }

        private bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head");
        }

        private bool IsBody(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "body");
        }
    }
}
