using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class HighReshielCardController : DjinnOngoingController
	{
		public HighReshielCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, "Reshiel", "Reshiel")
		{
		}

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger(dda => dda.DamageSource.Card == GetCardThisCardIsNextTo(), 1);
            base.AddTriggers();
        }

        public override Power GetGrantedPower(CardController cardController)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, $"{cardController.Card.Title} deals up to 3 targets 2 sonic damage each. Destroy {this.Card.Title}.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int targets = GetPowerNumeral(0, 3);
            int damages = GetPowerNumeral(1, 2);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, card), damages, DamageType.Sonic, targets, false, 0,
                allowAutoDecide: true,
                cardSource: cs);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            //when played via discard, I'll already be in the trash, so skip
            if (!Card.IsInTrash)
            {
                coroutine = DestroySelf();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
