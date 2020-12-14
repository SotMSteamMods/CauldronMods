using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Cauldron.Echelon
{
    public class DatabaseUplinkCardController : EchelonBaseCardController
    {
        //==============================================================
        // At the end of your turn, you may discard a Tactic.
        // If you do, draw a card.
        // Power: Put a Tactic from your hand into play.
        //==============================================================

        public static string Identifier = "DatabaseUplink";

        private const int CardsToPlay = 1;

        public DatabaseUplinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new[] { HeroTurnTaker.Hand }, new LinqCardCriteria(IsTactic, "tactic"));
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDiscardResponse,
                TriggerType.DealDamage);

            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Put a Tactic from your hand into play.
            int powerNumeral = GetPowerNumeral(0, CardsToPlay);

            //IEnumerator routine = base.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false,
                //cardCriteria: new LinqCardCriteria(IsTactic));

            IEnumerator routine = base.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, powerNumeral, false, powerNumeral, new LinqCardCriteria(IsTactic));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator EndOfTurnDiscardResponse(PhaseChangeAction _)
        {
            // At the end of your turn, you may discard a Tactic.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator routine = base.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, true, 0, storedResults,
                cardCriteria: new LinqCardCriteria(IsTactic), cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // If you do, draw a card.
            if (!base.DidDiscardCards(storedResults, 1))
            {
                yield break;
            }

            routine = base.DrawCard(this.HeroTurnTaker);
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