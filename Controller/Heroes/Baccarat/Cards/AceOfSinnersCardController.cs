using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AceOfSinnersCardController : CardController
    {
        #region Constructors

        public AceOfSinnersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Increase damage dealt by hero targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsHero, 1, null, null, false);

            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck or this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.ShuffleTwoSameCardFromTrashOrDestroyResponse), new TriggerType[] { TriggerType.ShuffleCardIntoDeck, TriggerType.DestroySelf }, null, false);
        }

        private IEnumerator ShuffleTwoSameCardFromTrashOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            MoveCardDestination obj = new MoveCardDestination(base.TurnTaker.Deck, true, false, false);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            //...shuffle 2 cards with the same name from your trash into your deck...
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c), "two cards with the same name", true, false, null, null, false), obj.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true, null, null, base.GetCardSource(null));
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "two cards with the same name", true, false, null, null, false), obj.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }//...or this card is destroyed.
            else
            {
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
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

        private bool TwoOrMoreCopiesInTrash(Card c)
        {
            int num = (from card in base.TurnTaker.Trash.Cards
                       where card.Identifier == c.Identifier
                       select card).Count<Card>();
            return num >= 2;

        }

        #endregion Methods
    }
}