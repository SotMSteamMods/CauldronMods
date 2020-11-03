using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AbyssalSolitareCardController : CardController
    {
        #region Constructors

        public AbyssalSolitareCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
            reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
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
            coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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

       #endregion Methods
    }
}