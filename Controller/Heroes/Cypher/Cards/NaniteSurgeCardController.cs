using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class NaniteSurgeCardController : CypherBaseCardController
    {
        //==============================================================
        // You may draw a card.
        // You may play a card.
        // Each augmented hero regains X HP, where X is the number of Augments next to them.
        //==============================================================

        public static string Identifier = "NaniteSurge";

        public NaniteSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // You may draw a card.
            IEnumerator routine = base.DrawCard(null, true, allowAutoDraw: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // You may play a card.
            routine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Each augmented hero regains X HP, where X is the number of Augments next to them.
            routine = base.GameController.GainHP(base.HeroTurnTakerController, IsAugmented, 
                c => GetAugmentsForHero(c).Count, cardSource: GetCardSource());

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