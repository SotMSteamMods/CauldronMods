using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public abstract class EnclosureCardController : MenagerieCardController
    {
        protected EnclosureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.Card.UnderLocation.OverrideIsInPlay = false;

        }
        public override void AddTriggers()
        {
            //Front: Cards beneath villain cards are not considered in play. When an enclosure leaves play, put it under [Menagerie], discarding all cards beneath it. Put any discarded targets into play.
            //Back: Cards beneath enclosures are not considered in play. When an enclosure leaves play, discard all cards beneath it.
            base.AddBeforeLeavesPlayAction(this.HandleEnclosureCardsResponse, TriggerType.MoveCard);
            //Back: Heroes with enclosures in their play area may not damage cards in other play areas.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => base.CharacterCard.IsFlipped && action.DamageSource.Owner == this.GetEnclosedHero() && action.DamageSource != null && action.DamageSource.Card != null && action.Target.Location.OwnerTurnTaker != action.DamageSource.Card.Location.OwnerTurnTaker);
            base.AddTriggers();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            if (base.CharacterCard.IsFlipped)
            {
                //Back: When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. Heroes with enclosures in their play area may not damage cards in other play areas.
                List<TurnTaker> heroes = new List<TurnTaker>();
                int maxEnclosures = 5;
                foreach (TurnTaker hero in base.Game.HeroTurnTakers)
                {
                    int numEnclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsEnclosure(c) && c.Location.OwnerTurnTaker == hero && c.IsInPlayAndHasGameText)).Count();
                    if (numEnclosures < maxEnclosures)
                    {
                        maxEnclosures = numEnclosures;
                    }
                }
                foreach (TurnTaker hero in base.Game.HeroTurnTakers)
                {
                    int numEnclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsEnclosure(c) && c.Location.OwnerTurnTaker == hero && c.IsInPlayAndHasGameText)).Count();
                    if (numEnclosures == maxEnclosures)
                    {
                        heroes.Add(hero);
                    }
                }
                IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && heroes.Contains(c.Owner) && !c.IsIncapacitatedOrOutOfGame), storedResults, isPutIntoPlay, decisionSources);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                if (storedResults != null && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(this.TurnTaker)))
                {
                    storedResults.Add(new MoveCardDestination(this.TurnTaker.PlayArea));
                }
            }
            yield break;
        }

        private IEnumerator HandleEnclosureCardsResponse(GameAction gameAction)
        {
            //make message
            string message = $"{CharacterCard.Title} discards {base.Card.UnderLocation.NumberOfCards.ToString_NumberOrNo()} cards from under {Card.Title}";
            if (!base.CharacterCard.IsFlipped)
            {
                int targets = Card.UnderLocation.Cards.Where(c => c.MaximumHitPoints.HasValue).Count();
                message += $" and puts {targets.ToString_NumberOrNo()} {targets.ToString_SingularOrPlural("target", "targets")} into play";
            }
            message += ".";

            IEnumerator coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new[] { CharacterCard });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...discarding all cards beneath it. 
            while (base.Card.UnderLocation.HasCards)
            {
                Card topCard = base.Card.UnderLocation.TopCard;
                if (topCard.IsFlipped)
                {
                    coroutine = base.GameController.FlipCard(base.FindCardController(topCard), cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                Location destination = topCard.Owner.Trash;
                bool isPutIntoPlay = false;
                //Front: Put any discarded targets into play.
                if (topCard.MaximumHitPoints.HasValue && !base.CharacterCard.IsFlipped)
                {
                    destination = topCard.Owner.PlayArea;
                    isPutIntoPlay = true;
                }
                coroutine = base.GameController.MoveCard(base.TurnTakerController, topCard, destination, isPutIntoPlay: isPutIntoPlay, isDiscard: !isPutIntoPlay, cardSource: base.GetCardSource());
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

        public IEnumerator EncloseTopCardResponse(Location source)
        {
            //When this card enters play, place the top card of the villain deck beneath it face down.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, source.TopCard, base.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
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

        private TurnTaker GetEnclosedHero()
        {
            if (base.Card.Location.OwnerCard != null && IsHero(base.Card.Location.OwnerCard))
            {
                return base.Card.Location.OwnerCard.Owner;
            }
            return null;
        }
    }
}