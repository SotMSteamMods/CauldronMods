using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public abstract class TangoOneBaseCardController : CardController
    {
        protected static readonly string IncreaseDamageIdentifier = "TangoOneIncreaseDamageId";

        protected TangoOneBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsCritical(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "critical");
        }

        protected bool IsOwnCharacterCard(Card card)
        {
            return card == CharacterCard;
        }

        protected IEnumerator SelectOwnCharacterCard(List<SelectCardDecision> results, SelectionType selectionType)
        {
            /*
             * Tango One doesn't have any multi-card promos, so this should never be necessary
             * (whatever card-replacement effect is happening should handle the swap automatically)
             * but I'm leaving it in for the sake of not disturbing too much.
             * */
            if (base.HeroTurnTakerController.HasMultipleCharacterCards)
            {
                LinqCardCriteria criteria = new LinqCardCriteria(IsOwnCharacterCard, "hero character cards");
                var routine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, selectionType,
                    criteria, results, false, cardSource: base.GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            else
            {
                var result = new SelectCardDecision(this.GameController, this.DecisionMaker, selectionType,
                    new[] { base.CharacterCard }, false, true, cardSource: base.GetCardSource());
                result.ChooseIndex(0);
                result.AutoDecide();
                results.Add(result);
            }
            yield break;
        }
    }
}