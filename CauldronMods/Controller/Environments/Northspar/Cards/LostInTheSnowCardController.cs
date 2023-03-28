using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class LostInTheSnowCardController : NorthsparCardController
    {

        public LostInTheSnowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 1 cold damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => IsHeroTarget(c), TargetType.All, 1, DamageType.Cold);

            //At the start of the environment turn, each hero must destroy 1 ongoing and 1 equipment card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyOngoingOrEquipmentResponse, TriggerType.DestroyCard);
        }

        private bool VisibleHeroTurntakersQuery(TurnTaker tt)
        {
            return IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource());
        }

        private IEnumerator DestroyOngoingOrEquipmentResponse(PhaseChangeAction pca)
        {
            //each hero must destroy 1 ongoing and 1 equipment card.
            IEnumerator ongoing = EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria(VisibleHeroTurntakersQuery, "heroes with ongoing or equipment cards in play"), new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"));
            IEnumerator equipment = EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria(VisibleHeroTurntakersQuery, "heroes with ongoing or equipment cards in play"), new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(ongoing);
                yield return base.GameController.StartCoroutine(equipment);

            }
            else
            {
                base.GameController.ExhaustCoroutine(ongoing);
                base.GameController.ExhaustCoroutine(equipment);
            }
        }
    }
}