using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public abstract class TiamatCharacterCardController : VillainCharacterCardController
    {
        #region Constructors

        public TiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties 

        public override bool CanBeDestroyed
		{
			get
			{
				return false;
			}
		}

		#endregion Properties 

		#region Methods

		//When a {Tiamat} head is destroyed, flip her.
		public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
		{
			FlipCardAction action = new FlipCardAction(base.GameController, this, false, false, destroyCard.ActionSource);
			IEnumerator coroutine = base.DoAction(action);
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
		public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
		{
			CardSource cardSource = flip.CardSource;
			if (cardSource == null && flip.ActionSource != null)
			{
				cardSource = flip.ActionSource.CardSource;
			}
			if (cardSource == null)
			{
				cardSource = base.GetCardSource(null);
			}
			IEnumerator coroutine = base.GameController.RemoveTarget(base.Card, true, cardSource);
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