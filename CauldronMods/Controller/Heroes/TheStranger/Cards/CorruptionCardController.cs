using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class CorruptionCardController : TheStrangerBaseCardController
    {
        #region Constructors

        public CorruptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //Draw up to 4 cards.
            List<DrawCardAction> storedResultsDraw = new List<DrawCardAction>();
            IEnumerator coroutine = base.DrawCards(this.DecisionMaker, 4, false, true, storedResultsDraw, false, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //For each card drawn this way, {TheStranger} deals himself 1 toxic damage.

            int numberOfCardsDrawn = base.GetNumberOfCardsDrawn(storedResultsDraw);
            for (int i = 0; i < numberOfCardsDrawn; i++)
            {
                IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Toxic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

        }
        #endregion Methods
    }
}