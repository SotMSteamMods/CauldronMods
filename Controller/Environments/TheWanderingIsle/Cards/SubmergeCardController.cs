using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.TheWanderingIsle
{
    public class SubmergeCardController : TheWanderingIsleCardController
    {
        public SubmergeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Reduce all damage dealt by 2
            base.AddReduceDamageTrigger((Card c) => true, 2);
            //At the start of the environment turn, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {

            //When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocations(new Location[]
            {
                base.TurnTaker.Deck,
                base.TurnTaker.Trash
            }, "Teryx", true, null, false, true, true);
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
