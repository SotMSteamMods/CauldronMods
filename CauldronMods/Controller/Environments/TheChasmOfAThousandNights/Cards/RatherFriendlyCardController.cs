using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class RatherFriendlyCardController : NatureCardController
    {
        public RatherFriendlyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildHighestVillainTargetsSpecialString(), relatedCards: GetCardThisCardIsBelow().ToEnumerable).Condition = () => GetCardThisCardIsBelow() != null;
        }

        public override void AddTriggers()
        {
            //Redirect all damage dealt by the target next to this card to the villain target with the highest HP.
            AddTrigger((DealDamageAction dd) => GetCardThisCardIsBelow() != null && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == GetCardThisCardIsBelow(), RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            base.AddTriggers();
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dealDamage)
        {
            IEnumerator coroutine = RedirectDamage(dealDamage, TargetType.HighestHP, (Card c) => IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()));

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

        private string BuildHighestVillainTargetsSpecialString()
        {

            IEnumerable<Card> highestVillainTargets = GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => IsVillainTarget(c), cardSource: GetCardSource());
            string highestHPSpecial = $"Villain targets with the highest HP: ";
            if (highestVillainTargets.Any())
            {
                highestHPSpecial += string.Join(", ", highestVillainTargets.Select(c => c.Title).ToArray());
            }
            else
            {
                highestHPSpecial += "None";
            }

            return highestHPSpecial;

        }
    }
}
