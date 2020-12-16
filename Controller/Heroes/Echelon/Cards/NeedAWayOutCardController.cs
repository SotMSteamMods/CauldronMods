using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class NeedAWayOutCardController : CardController
    {
        //==============================================================
        // At the start of a player's turn, that player may choose to skip their play Phase.
        // If they do, they may use 1 additional power during their power Phase.
        // At the start of your turn, destroy this card and each hero target regains 1HP.
        //==============================================================

        public static string Identifier = "NeedAWayOut";

        private const int HpGain = 1;

        public NeedAWayOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the start of a player's turn, that player may choose to skip their play Phase.
            // If they do, they may use 1 additional power during their power Phase.


            // At the start of your turn, destroy this card and each hero target regains 1HP.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, DestroyAndHealResponse, new[]
            {
                TriggerType.GainHP,
                TriggerType.DestroySelf
            });

            base.AddTriggers();
        }

        private IEnumerator DestroyAndHealResponse(PhaseChangeAction pca)
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, storedResults: storedResults,
                cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!storedResults.Any() || !base.DidDestroyCard(storedResults.First()))
            {
                yield break;
            }

            // each hero target regains 1 HP
            routine = base.GameController.GainHP(this.HeroTurnTakerController,
                c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HpGain, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}