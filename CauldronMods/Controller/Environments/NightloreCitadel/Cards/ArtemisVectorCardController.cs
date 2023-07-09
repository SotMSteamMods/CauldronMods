using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class ArtemisVectorCardController : NightloreCitadelUtilityCardController
    {
        public ArtemisVectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(c => c.IsInPlayAndHasGameText && IsVillainTarget(c) && c.NextToLocation.Cards.Any(c2 => IsConstellation(c2)), "villain targets with constellations next to them", useCardsSuffix: false));
        }

        public override void AddTriggers()
        {
            // Increase damage dealt to villain targets with Constellations next to them by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target != null && IsVillainTarget(dd.Target) && dd.Target.NextToLocation.Cards.Any(c => IsConstellation(c)), 1);
            // When this card is destroyed, each hero target deals themselves 2 psychic damage and each player draws a card.
            AddWhenDestroyedTrigger(WhenDestroyedResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DrawCard
            });
        }

        public IEnumerator WhenDestroyedResponse(DestroyCardAction dca)
        {
            //each hero target deals themselves 2 psychic damage
            IEnumerator coroutine = GameController.DealDamageToSelf(DecisionMaker, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 2, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //each player draws a card
            coroutine = EachPlayerDrawsACard();
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
    }
}
