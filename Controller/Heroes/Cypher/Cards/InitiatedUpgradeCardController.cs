using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class InitiatedUpgradeCardController : CypherBaseCardController
    {
        //==============================================================
        // Search your deck or trash for an Augment card and put it into play.
        // If you searched your deck, shuffle it.
        // You may draw a card.
        //==============================================================

        public static string Identifier = "InitiatedUpgrade";

        private const int CardsToPlay = 1;

        public InitiatedUpgradeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Search your deck or trash for an Augment card and put it into play.
            IEnumerator routine = base.SearchForCards(base.HeroTurnTakerController, true, true, CardsToPlay, CardsToPlay, 
                new LinqCardCriteria(IsAugment, "augment"), true, false, false);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // You may draw a card.
            routine = base.DrawCard(this.HeroTurnTaker, optional: true, allowAutoDraw: false);
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