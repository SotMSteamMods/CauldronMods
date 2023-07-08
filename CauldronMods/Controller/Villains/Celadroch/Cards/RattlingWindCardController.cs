using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class RattlingWindCardController : CeladrochOngoingCardController
    {
        /*
         * 	"When this card enters play, play the top card of the villain deck.",
			"Whenever a hero draws a card, {Celadroch} deals them 1 projectile damage.",
			"When this card is destroyed, {Celadroch} deals each hero target 1 cold damage."
         */

        public RattlingWindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            return base.Play();
        }

        public override void AddTriggers()
        {
            AddTrigger<DrawCardAction>(dca => dca.DidDrawCard, DrawCardDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

            AddWhenDestroyedTrigger(ga => GameController.DealDamage(DecisionMaker, CharacterCard, c => IsHeroTarget(c), 1, DamageType.Cold, cardSource: GetCardSource()), TriggerType.DealDamage);
        }

        private IEnumerator DrawCardDamageResponse(DrawCardAction dca)
        {
            var result = new List<Card>();
            var coroutine = FindCharacterCardToTakeDamage(dca.HeroTurnTaker, result, CharacterCard, 1, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var card = result.First();
            if (card != null)
            {
                coroutine = DealDamage(CharacterCard, card, 1, DamageType.Projectile, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }


    }
}