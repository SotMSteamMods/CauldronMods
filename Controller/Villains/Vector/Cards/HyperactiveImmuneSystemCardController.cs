using System.Collections;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class HyperactiveImmuneSystemCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, {Vector} regains X HP,
        // where X is the number of hero Ongoing and Equipment cards in play.
        //==============================================================

        public static readonly string Identifier = "HyperactiveImmuneSystem";

        public HyperactiveImmuneSystemCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Owner.IsHero && c.IsInPlayAndNotUnderCard && (c.IsOngoing || IsEquipment(c)), "hero Ongoing and Equipment"));
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, GainHpResponse, TriggerType.GainHP);

            base.AddTriggers();
        }

        private IEnumerator GainHpResponse(PhaseChangeAction pca)
        {
            int eligibleCards = FindCardsWhere(c => c.Owner.IsHero && c.IsInPlayAndNotUnderCard && (c.IsOngoing || IsEquipment(c))).Count();

            IEnumerator routine = this.GameController.GainHP(this.CharacterCard, eligibleCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}