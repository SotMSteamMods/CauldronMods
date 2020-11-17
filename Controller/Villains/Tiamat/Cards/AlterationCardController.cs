﻿using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class AlterationCardController : CardController
    {
        #region Constructors

        public AlterationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
			//Play the top card of the environment deck. Play the top card of the villain deck.
			IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
			IEnumerator coroutine2 = base.GameController.PlayTopCard(this.DecisionMaker, base.FindEnvironment(), cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}

        #endregion Methods
    }
}