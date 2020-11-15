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
        public AceOfSinnersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c)));
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by hero targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.IsHero, 1);

            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck or this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.ShuffleTwoSameCardFromTrashOrDestroyResponse), new TriggerType[] { TriggerType.ShuffleCardIntoDeck, TriggerType.DestroySelf });
        }

        private IEnumerator ShuffleTwoSameCardFromTrashOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            MoveCardDestination obj = new MoveCardDestination(base.TurnTaker.Deck, true);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            //...shuffle 2 cards with the same name from your trash into your deck...
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c), "two cards with the same name"), obj.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "card with the same name", false);
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                //Move second card
                coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "two cards with the same name"), obj.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //actually shuffle deck
                coroutine = base.GameController.ShuffleLocation(this.TurnTaker.Deck);
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
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card);
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
    }
}