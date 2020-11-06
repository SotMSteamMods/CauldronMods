using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    class InfernoTiamatCharacterCardController : TiamatCharacterCardController
	{
        public InfernoTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		private bool IsSpell(Card card)
        {
			return card != null && base.GameController.DoesCardContainKeyword(card, "spell", false, false);
		}
		private bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
		}

		public override void AddSideTriggers()
        {
			//Front - The Mouth of The Inferno
            if (!base.Card.IsFlipped)
			{
				//{Tiamat}, The Mouth of the Inferno is immune to fire damage.
				base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageType == DamageType.Fire, false));

				//At the end of the villain turn, if {Tiamat}, The Mouth of the Inferno dealt no damage this turn, she deals the hero target with the highest HP {H - 2} fire damage.
				base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, (PhaseChangeAction p) => this.DidInfernoDealDamageThisTurn(), false));
				
				//If advanced
				if (base.IsGameAdvanced)
				{
					//Increase damage dealt by {Tiamat}, The Mouth of the Inferno by 1.
					this.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsCard && dealDamage.DamageSource.Card == base.CharacterCard, 1, null, null, false));
				}
			}
			//Back - Decapitated
			else
			{
				//When a spell card causes a head to deal damage, increase that damage by 1 for each “Element of Fire“ card in the villain trash.
				this.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => IsSpell(dealDamage.CardSource.Card) && IsHead(dealDamage.DamageSource.Card), GetNumberOfElementOfFireInTrash()));
				base.AddSideTrigger(base.AddCannotDealDamageTrigger((Card c) => c == base.Card));


				if (base.IsGameAdvanced)
                {
					//Reduce damage dealt by heads by 1.
					this.AddSideTrigger(base.AddReduceDamageTrigger((Card c) => IsHead(c),1));
				}
			}
		}

		//Did Inferno Deal Damage This Turn
		private bool DidInfernoDealDamageThisTurn()
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

		//Deal H-2 Fire damage to highest hero target
		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(base.H - 2), DamageType.Fire, false, false, null, null, null, null, false, false);
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

		//Get number of "Element of Fire" cards in trash
		private int GetNumberOfElementOfFireInTrash()
        {
			return (from card in base.TurnTaker.Trash.Cards
						  where card.Identifier == "ElementOfFire"
						  select card).Count<Card>();
        }
	}
}
