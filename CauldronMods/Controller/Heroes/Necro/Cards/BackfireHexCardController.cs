using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Necro
{
    public class BackfireHexCardController : NecroCardController
    {
        public BackfireHexCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria(c => this.IsUndead(c), "undead"));
        }
        public override IEnumerator Play()
        {
            //You may destroy an ongoing card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlay && IsOngoing(c), "ongoing"), true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Put an undead card from the trash into play.

            var destinations = new MoveCardDestination(base.TurnTaker.PlayArea).ToEnumerable();
            IEnumerator coroutine2 = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria(c => this.IsUndead(c), "undead"), destinations,
                isPutIntoPlay: true,
                cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}
