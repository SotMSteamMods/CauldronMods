using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class FeedingTimeCardController : MenagerieCardController
    {
        public FeedingTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsSpecimen(c), "specimen"));
        }

        public override IEnumerator Play()
        {
            //When this card enters play, put all Mercenary cards from the villain trash into play.
            IEnumerator coroutine = base.PlayCardsFromLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => base.IsMercenary(c), "mercenary"), true);
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

        public override void AddTriggers()
        {
            //Reduce damage dealt to Mercenaries by X, where X is the number of Specimens in play.
            base.AddReduceDamageTrigger((DealDamageAction action) => base.IsMercenary(action.Target), (DealDamageAction action) => base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsSpecimen(c) && c.IsInPlayAndHasGameText, "specimen")).Count());
            base.AddTriggers();
        }
    }
}