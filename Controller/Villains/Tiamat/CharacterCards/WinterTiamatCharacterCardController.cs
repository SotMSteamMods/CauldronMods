using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    class StormTiamatCharacterCardController : VillainCharacterCardController
    {
        public StormTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
			//Front - The Mouth of The Storm
            if (!base.Card.IsFlipped)
			{
				//{Tiamat}, The Mouth of the Storm is immune to Lightning damage.
				base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageType == DamageType.Lightning, false));

				//At the end of the villain turn, if {Tiamat}, The Mouth of the Storm dealt no damage this turn, she deals the hero target with the highest HP {H - 2} Lightning damage.
				base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, (PhaseChangeAction p) => this.DidStormDealDamageThisTurn(), false));
				
				//If advanced
				if (base.IsGameAdvanced)
				{
					//Increase damage dealt by {Tiamat}, The Mouth of the Storm by 1.
					this.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsCard && dealDamage.DamageSource.Card == base.CharacterCard, 1, null, null, false));
				}
			}
			//Back - Decapitated
			else
			{
				//When a spell card causes a head to deal damage, increase that damage by 1 for each “Element of Lightning“ card in the villain trash.
				this.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => IsSpell(dealDamage.CardSource.Card) && IsHead(dealDamage.DamageSource.Card), GetNumberOfElementOfLightningInTrash()));


				if (base.IsGameAdvanced)
                {
					//Reduce damage dealt by heads by 1.
					this.AddSideTrigger(base.AddReduceDamageTrigger((Card c) =>  IsHead(c),1));
				}
			}
		}

		//When {Tiamat}, Mouth of the Storm is destroyed, flip her.
		public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
		{
			if (!base.Card.IsFlipped)
			{
				IEnumerator coroutine = base.GameController.RemoveTarget(base.Card, true, base.GetCardSource(null));
				IEnumerator e2 = base.GameController.FlipCard(this, false, false, null, null, base.GetCardSource(null), true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
					yield return base.GameController.StartCoroutine(e2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
					base.GameController.ExhaustCoroutine(e2);
				}

				if (base.CharacterCard.IsInPlayAndHasGameText)
				{
					if (base.GameController.IsCardIndestructible(base.CharacterCard) || base.CharacterCard.HitPoints == null)
					{
						goto IL_1BC;
					}
					int? hitPoints = base.CharacterCard.HitPoints;
					int num = 0;
					if (!(hitPoints.GetValueOrDefault() <= num & hitPoints != null))
					{
						goto IL_1BC;
					}
				}

				IEnumerator coroutine2 = base.GameController.GameOver(EndingResult.AlternateVictory, "Tiamat has been defeated!", false, null, null, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
				IL_1BC:
				e2 = null;
			}
			yield break;
		}

		//Did Storm Deal Damage This Turn
		private bool DidStormDealDamageThisTurn()
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

		//Deal H-2 Lightning damage to highest hero target
		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(base.H - 2), DamageType.Lightning, false, false, null, null, null, null, false, false);
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

		//Get number of "Element of Lightning" cards in trash
		private int GetNumberOfElementOfLightningInTrash()
        {
			return (from card in base.TurnTaker.Trash.Cards
						  where card.Identifier == "ElementOfLightning"
						  select card).Count<Card>();
        }
	}
}
