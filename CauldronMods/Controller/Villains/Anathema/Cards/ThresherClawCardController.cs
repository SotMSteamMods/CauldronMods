using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class ThresherClawCardController : ArmCardController
    {
		public ThresherClawCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroCharacterCardWithHighestHP(numberOfTargets: Game.H - 2);
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals the {H-2} heroes with the highest HP 3 toxic damage each.
			base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.CharacterCard, (Card c) =>  IsHeroCharacterCard(c), TargetType.HighestHP, 3, DamageType.Toxic, numberOfTargets: base.H - 2);
		}

	}
}
