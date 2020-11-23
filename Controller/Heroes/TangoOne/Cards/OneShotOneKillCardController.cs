using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class OneShotOneKillCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Discard any number of cards. Destroy a target with X or fewer HP,
        // where X is 2 times the number of cards discarded this way.
        // If you destroyed a target this way, draw a card.
        //==============================================================

        public static string Identifier = "OneShotOneKill";

        private const int HpMultiplier = 2;

        public OneShotOneKillCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.HeroTurnTaker.Hand);
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> discardCardActions = new List<DiscardCardAction>();
            IEnumerator discardCardsRoutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, null, false, new int?(0), discardCardActions);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardsRoutine);
            }

            if (!discardCardActions.Any())
            {
                yield break;
            }

            // If we got this far, at least one card was selected

            // Determine the max HP of eligible targets
            int maxHpThresholdForTarget = discardCardActions.Count * HpMultiplier;

            LinqCardCriteria cardCriteria = new LinqCardCriteria(card => card.HitPoints <= maxHpThresholdForTarget && card.IsTarget && card.IsInPlayAndHasGameText);
            List<DestroyCardAction> destroyCardActions = new List<DestroyCardAction>();

            IEnumerator destroyCardRoutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, cardCriteria, true,
                destroyCardActions, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }

            if (!destroyCardActions.Any())
            {
                yield break;
            }

            // A card was destroyed, draw a card
            IEnumerator drawCardRoutine = this.DrawCard(this.HeroTurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
            }
        }
    }
}