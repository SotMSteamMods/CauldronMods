using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class TheStuffOfNightmaresCardController : CardController
    {
		public TheStuffOfNightmaresCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{

			//Whenever a Villain target enters play, that target deals the hero target with the second lowest HP 2 psychic damage.

			base.AddTargetEntersPlayTrigger((Card c) => c.IsVillainTarget, (Card c) => base.DealDamageToLowestHP(c,2,(Card h) => h.IsHero,(Card n) => new int?(2),DamageType.Psychic,false,false,null,1,null,null,false), TriggerType.DealDamage, TriggerTiming.After, false, false);
		}

	
		

	}
}
