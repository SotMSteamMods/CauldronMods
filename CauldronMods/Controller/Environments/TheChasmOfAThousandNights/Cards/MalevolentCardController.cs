using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class MalevolentCardController : NatureCardController
    {
        public MalevolentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowHeroTargetWithHighestHP();
            ss.Condition = () => GetCardThisCardIsBelow() != null;
            ss.RelatedCards = () => new[] { GetCardThisCardIsBelow() };
        }

        public override void AddTriggers()
        {
            //Redirect all damage dealt by the target next to this card to the hero target with the highest HP.
            AddTrigger((DealDamageAction dd) => !IsIreOfTheDjinnInPlay() && GetCardThisCardIsBelow() != null && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == GetCardThisCardIsBelow(), RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);

            base.AddTriggers();
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dealDamage)
        {
            IEnumerator coroutine = RedirectDamage(dealDamage, TargetType.HighestHP, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
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
