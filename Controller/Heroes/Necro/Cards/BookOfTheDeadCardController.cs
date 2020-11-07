using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Necro
{
    public class BookOfTheDeadCardController : NecroCardController
    {
        public BookOfTheDeadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new Location[]
            {
                base.TurnTaker.Trash,
                base.TurnTaker.Deck
            }, new LinqCardCriteria(c => this.IsRitual(c), "ritual"));
        }
        public override IEnumerator Play()
        {
            //Search your deck or trash for a ritual and put it into play or into your hand. If you searched your deck, shuffle your deck.
            IEnumerator coroutine = base.SearchForCards(base.HeroTurnTakerController, true, true, 1, 1, new LinqCardCriteria(c => this.IsRitual(c), "ritual"), true, true, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may draw a card.
            IEnumerator coroutine3 = base.DrawCard(optional: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }
    }
}
