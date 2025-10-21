using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LaughingHagCardController : CardController
    {
        /*
         *  "At the end of the villain turn, destroy 1 hero ongoing or equipment card.",
			"This card is immune to fire, lightning, cold, and toxic damage.",
			"Increase damage dealt to hero targets by 1."
         */

        public LaughingHagCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => Card.Title + " is immune to fire, lightning, cold, and toxic damage.").Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            AddImmuneToDamageTrigger(dda => dda.Target == Card &&
                (dda.DamageType == DamageType.Fire || dda.DamageType == DamageType.Lightning || dda.DamageType == DamageType.Cold || dda.DamageType == DamageType.Toxic));

            AddIncreaseDamageTrigger(dda => IsHeroTarget(dda.Target), 1);

            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DestroyCardResponse(), TriggerType.DestroyCard);
        }

        private IEnumerator DestroyCardResponse()
        {
            return GameController.SelectAndDestroyCard(DecisionMaker,
                        new LinqCardCriteria(c => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), false,
                        cardSource: GetCardSource());
        }
    }
}
