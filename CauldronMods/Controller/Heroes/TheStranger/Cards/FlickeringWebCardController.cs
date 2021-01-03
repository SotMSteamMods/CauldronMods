using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class FlickeringWebCardController : CardController
    {
        #region Constructors

        public FlickeringWebCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.Hand, new LinqCardCriteria(c => IsRune(c), "rune", false, false, null, "runes"));
        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //You may play up to 3 Runes now
            IEnumerator coroutine = base.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 3, false, new int?(0), new LinqCardCriteria((Card c) => this.IsRune(c), "rune"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
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

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }
        #endregion Methods
    }
}