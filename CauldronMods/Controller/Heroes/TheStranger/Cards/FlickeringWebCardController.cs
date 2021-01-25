using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class FlickeringWebCardController : TheStrangerBaseCardController
    {
        public FlickeringWebCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.Hand, IsRuneCriteria());
        }

        public override IEnumerator Play()
        {
            //You may play up to 3 Runes now
            IEnumerator coroutine = base.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 3, false, 0, IsRuneCriteria());
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
    }
}