using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AbyssalSolitaireCardController : CardController
    {
        public AbyssalSolitaireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
            reduceDamageStatusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}