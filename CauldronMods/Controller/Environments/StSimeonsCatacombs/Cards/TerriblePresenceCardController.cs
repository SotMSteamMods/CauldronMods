using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class TerriblePresenceCardController : StSimeonsGhostCardController
    {
        public static readonly string Identifier = "TerriblePresence";

        public TerriblePresenceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { TortureChamberCardController.Identifier, AqueductsCardController.Identifier }, false)
        {
            SpecialStringMaker.ShowLowestHP(numberOfTargets: () => 2, cardCriteria: new LinqCardCriteria(c => !IsGhost(c) && c.IsTarget, "", useCardsSuffix: false, singular: "non-ghost target", plural: "non-ghost targets"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the 2 non-ghost targets with the lowest HP 2 cold damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.AddEndOfTurnResponse), TriggerType.DealDamage);
        }

        private IEnumerator AddEndOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals the 2 non-ghost targets with the lowest HP 2 cold damage each
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => !this.IsGhost(c), (Card c) => 2, DamageType.Cold, numberOfTargets: 2);
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