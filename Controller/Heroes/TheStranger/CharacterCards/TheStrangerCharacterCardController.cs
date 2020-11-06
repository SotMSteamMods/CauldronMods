using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.TheStranger
{
	public class TheStrangerCharacterCardController : HeroCharacterCardController
	{
		public TheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Play a rune.
			yield break;
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//One player may draw a card now.
						break;
					}
				case 1:
					{
						//Reveal the top card of a deck, then replace it or discard it.
						break;

					}
				case 2:
					{
						//Up to 2 ongoing hero cards may be played now.
						break;
					}
			}
			yield break;
		}

		private bool IsRune(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
		}
	}
}
