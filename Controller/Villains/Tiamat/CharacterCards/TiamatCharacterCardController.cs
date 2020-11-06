using System;
using System.Collections;
using System.Linq;
using Handelabra;
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

		protected abstract ITrigger[] AddFrontTriggers();
		protected abstract ITrigger[] AddFrontAdvancedTriggers();
		protected abstract ITrigger[] AddDecapitatedTriggers();
		protected abstract ITrigger[] AddDecapitatedAdvancedTriggers();

		#endregion Properties 

		#region Methods

		public bool IsSpell(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "spell", false, false);
		}
		public bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
		}

		public override void AddSideTriggers()
        {
			if (!base.Card.IsFlipped)
			{
				base.AddSideTriggers(this.AddFrontTriggers());
                if (Game.IsAdvanced)
                {
					base.AddSideTriggers(this.AddFrontAdvancedTriggers());
                }
			}
			else
			{
				base.AddSideTriggers(this.AddDecapitatedTriggers());
				base.AddSideTrigger(base.AddCannotDealDamageTrigger((Card c) => c == base.Card));
				if (Game.IsAdvanced)
				{
					base.AddSideTriggers(this.AddDecapitatedAdvancedTriggers());
				}
			}
		}

		//Did Inferno Deal Damage This Turn
		public bool DidDealDamageThisTurn()
		{
			int result = 0;
			try
			{
				result = (from e in base.GameController.Game.Journal.DealDamageEntriesThisTurn()
						  where e.SourceCard == base.CharacterCard
						  select e.Amount).Sum();
			}
			catch (OverflowException ex)
			{
				Log.Warning("DamageDealtThisTurn overflowed: " + ex.Message);
				result = int.MaxValue;
			}
			return result == 0;
		}

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