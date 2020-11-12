using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cauldron.Anathema
{
	public class BiofeedbackCardController : CardController
    {
		public BiofeedbackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Whenever Anathema deals damage to a Hero target, he regains 1 HP.
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target.IsHero && dd.DamageSource.Card == base.CharacterCard, 
				(DealDamageAction dd) => base.GameController.GainHP(base.CharacterCard, new int?(1), null, null, base.GetCardSource(null)),
				new TriggerType[] { TriggerType.GainHP },TriggerTiming.After,null,false,true,null,false,null,null,false,false);

			//Whenever an arm, body, or head is destroyed by a Hero target, Anathema deals himself 2 psychic damage.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => this.IsArmHeadOrBody(dca.CardToDestroy.Card) && dca.CardSource.Card.IsTarget && dca.CardSource != null && dca.CardSource.Card.IsHero,
				(DestroyCardAction dca) => base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, false, false, false, null, null, null, false, base.GetCardSource(null)),
				new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After, null, false, false, null, false, null, null, false, false);
		}
		

		private bool IsArm(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "arm", false, false);
		}

		private bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
		}

		private bool IsBody(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "body", false, false);
		}

		private bool IsArmHeadOrBody(Card c)
		{
			return IsArm(c) || IsHead(c) || IsBody(c);
		}



	}
}
