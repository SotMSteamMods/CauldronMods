using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class TangoOneBaseCardController : CardController
    {
        public TangoOneBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsCritical(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "critical");
        }

        protected bool IsOwnCharacterCard(Card card)
        {
            return card.IsHeroCharacterCard && card.ParentDeck == this.Card.ParentDeck;
        }

        protected IEnumerator SelectOwnCharacterCard(List<SelectCardDecision> results, SelectionType selectionType)
        {
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