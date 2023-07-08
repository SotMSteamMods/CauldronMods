using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class RedSunXuCardController : TheInfernalChoirUtilityCardController
    {
        public RedSunXuCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
        }

        public readonly string GhostKeyword = "ghost";
        public readonly string EquipmentKeyword = "equipment";

        public override bool AskIfCardContainsKeyword(Card card, string keyword, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            if (keyword == GhostKeyword && card.DoKeywordsContain(EquipmentKeyword, evenIfUnderCard, evenIfFaceDown))
            {
                return true;
            }
            return base.AskIfCardContainsKeyword(card, keyword, evenIfUnderCard, evenIfFaceDown);
        }

        public override IEnumerable<string> AskForCardAdditionalKeywords(Card card)
        {
            if (card.DoKeywordsContain(EquipmentKeyword, true, true))
            {
                return new[] { GhostKeyword };
            }
            return base.AskForCardAdditionalKeywords(card);
        }

        public override IEnumerator Play()
        {
            var coroutine = base.GameController.MakeTargettable(DecisionMaker, (Card c) => IsEquipment(c), (Card c) => 1, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddBeforeLeavesPlayAction((GameAction d) => base.GameController.RemoveTargets((Card c) => IsEquipment(c), true, GetCardSource()), TriggerType.RemoveTarget);
            AddMaintainTargetTriggers((Card c) => IsEquipment(c), 1, new List<string>() { "equipment" });

            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => EndOfTurnDamage(), TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator EndOfTurnDamage()
        {
            return GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 3, DamageType.Psychic, H - 1, false, H - 1, additionalCriteria: c => IsHero(c), allowAutoDecide: true, cardSource: GetCardSource());
        }
    }
}
