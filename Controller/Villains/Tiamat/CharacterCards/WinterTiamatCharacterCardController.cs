using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    class WinterTiamatCharacterCardController : TiamatCharacterCardController
	{
        public WinterTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

		}

		protected override ITrigger[] AddFrontTriggers()
		{
			return new ITrigger[]
			{ 
				//{Tiamat}, The Jaws of Winter is immune to Cold damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Cold, false),
				//At the end of the villain turn, if {Tiamat}, The Jaws of Winter dealt no damage this turn, she deals the hero target with the highest HP {H - 2} Cold damage. 
				base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, (PhaseChangeAction p) => this.DidDealDamageThisTurn()) 
			};
		}

		protected override ITrigger[] AddFrontAdvancedTriggers()
		{
			return new ITrigger[]
			{
				//Increase damage dealt by {Tiamat}, The Jaws of Winter by 1.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.IsCard && dealDamage.DamageSource.Card == base.Card, 1)
			};
		}

		protected override ITrigger[] AddDecapitatedAdvancedTriggers()
		{
			return new ITrigger[]
			{
				//Reduce damage dealt by heads by 1.
				base.AddReduceDamageTrigger((Card c) => IsHead(c), 1)
			};
		}

		protected override ITrigger[] AddDecapitatedTriggers()
		{
			return new ITrigger[]
			{
				//When a spell card causes a head to deal damage, increase that damage by 1 for each “Element of Cold“ card in the villain trash.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.CardSource != null && IsSpell(dealDamage.CardSource.Card) && IsHead(dealDamage.DamageSource.Card), GetNumberOfElementOfIceInTrash()) 
			};
		}

		//Deal H-2 Cold damage to highest hero target
		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsTarget && c.IsHero, (Card c) => new int?(base.H - 2), DamageType.Cold);
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

		//Get number of "Element of Ice" cards in trash
		private int GetNumberOfElementOfIceInTrash()
        {
			return (from card in base.TurnTaker.Trash.Cards
						  where card.Identifier == "ElementOfIce"
						  select card).Count<Card>();
        }
	}
}
