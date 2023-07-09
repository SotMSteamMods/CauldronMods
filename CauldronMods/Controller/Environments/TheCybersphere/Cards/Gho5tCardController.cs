using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class Gho5tCardController : TheCybersphereCardController
    {

        public Gho5tCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each hero must destroy 1 of their ongoing or equipment cards.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyCardsResponse, TriggerType.DestroyCard);

            //When this card is destroyed, each player may draw a card.
            AddWhenDestroyedTrigger(DrawCardsResponse, TriggerType.DrawCard);
        }

        private IEnumerator DrawCardsResponse(DestroyCardAction dca)
        {
            //Each player may draw a card.
            IEnumerator coroutine = base.EachPlayerDrawsACard(heroCriteria: (HeroTurnTaker tt) => IsHero(tt) && !tt.ToHero().IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), optional: true);
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
        private IEnumerator DestroyCardsResponse(PhaseChangeAction pca)
        {
            //Each hero must destroy 1 of their ongoing or equipment cards.
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || IsEquipment(c)) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing or equipment");
            IEnumerator coroutine = base.EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt), "heroes"), cardCriteria, numberOfCards: new int?(1), requiredNumberOfCards: new int?(1));
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