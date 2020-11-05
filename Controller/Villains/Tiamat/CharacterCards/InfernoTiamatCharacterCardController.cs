using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    class InfernoTiamatCharacterCardController : VillainCharacterCardController
    {
        public InfernoTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddSideTriggers()
        {
			//Front - The Mouth of The Inferno
            if (!base.Card.IsFlipped)
			{
				//{Tiamat}, The Mouth of the Inferno is immune to fire damage.
				base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageType == DamageType.Fire, false));

				//At the end of the villain turn, if {Tiamat}, The Mouth of the Inferno dealt no damage this turn, she deals the hero target with the highest HP {H - 2} fire damage.

				//When {Tiamat}, Mouth of the Inferno is destroyed, flip her.
			}
			//Back - Decapitated
			else
			{

            }
		}

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
	}
}
