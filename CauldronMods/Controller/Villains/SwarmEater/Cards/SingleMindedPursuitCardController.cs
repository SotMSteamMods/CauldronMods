using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class SingleMindedPursuitCardController : CardController
    {
        public SingleMindedPursuitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.Card.Location.OwnerCard.Title + " is pursued.", null, () => new Card[]
            {
                base.Card.Location.OwnerCard
            }).Condition = (() => base.Card.Location.IsNextToCard);
        }

        public override IEnumerator Play()
        {
            if (base.CharacterCardController is SwarmEaterCharacterCardController && base.CharacterCard.IsFlipped)
            {
                //Whenever Single-Minded Pursuit enters play, flip {SwarmEater}'s villain character cards.
                IEnumerator coroutine = base.GameController.FlipCard(base.CharacterCardController, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //Increase damage dealt by {SwarmEater} by 2.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard, 2);
            //If the Pursued target leaves play, destroy this card.
            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the target with the lowest HP, other than {SwarmEater}. The target next to this card is Pursued.
            List<Card> lowestCard = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c != base.CharacterCard, lowestCard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card selectedTarget = lowestCard.FirstOrDefault<Card>();
            if (selectedTarget != null && storedResults != null)
            {
                //Play this card next to the target with the lowest HP, other than {SwarmEater}.
                storedResults.Add(new MoveCardDestination(selectedTarget.NextToLocation, false, false, false));
            }
            yield break;
        }
    }
}