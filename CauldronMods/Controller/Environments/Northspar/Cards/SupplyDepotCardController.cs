using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class SupplyDepotCardController : NorthsparCardController
    {

        public SupplyDepotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} is in {Card.Location.GetFriendlyName()}.").Condition = () => !Card.IsInPlay;
        }

        public override void AddTriggers()
        {
            base.AddAsPowerContributor();
        }
        public override IEnumerator Play()
        {
            //When this card enters play, destroy it and play the top card of the environment deck if Makeshift Shelter is not in play.Otherwise place it next to a hero.They gain:",
            //    "Power: this hero deals 1 target 1 fire damage."
            if (!this.IsMakeshiftShelterInPlay())
            {
                //When this card enters play, destroy it and play the top card of the environment deck if Makeshift Shelter is not in play.
                var coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card,
                                    postDestroyAction: () => GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource()),
                                    cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.Card == base.GetCardThisCardIsNextTo())
            {
                return new Power[]
                {
                    new Power(cardController.HeroTurnTakerController, cardController, "This hero deals 1 target 1 fire damage.", this.DealDamageResponse(),1,null,base.GetCardSource())
                };
            }
            return null;
        }

        private IEnumerator DealDamageResponse()
        {
            //this hero deals 1 target 1 fire damage.
            int targets = base.GetPowerNumeral(0, 1);
            int amount = base.GetPowerNumeral(1, 1);
            HeroTurnTakerController hero = base.FindHeroTurnTakerController(base.GetCardThisCardIsNextTo().Owner as HeroTurnTaker);
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(hero, new DamageSource(base.GameController, base.GetCardThisCardIsNextTo()), amount, DamageType.Fire, new int?(targets), false, new int?(targets), cardSource: base.GetCardSource());
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
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine;
            if (this.IsMakeshiftShelterInPlay())
            {
                coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "active hero"), storedResults, isPutIntoPlay, decisionSources);

            }
            else
            {
                coroutine = base.GameController.SelectMoveCardDestination(base.DecisionMaker, base.Card, new[] { new MoveCardDestination(base.TurnTaker.PlayArea) }, storedResults, cardSource: base.GetCardSource());
            }

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

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return base.IsThirdWaypoint(card);
        }

        private bool IsMakeshiftShelterInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "MakeshiftShelter").Count() > 0;
        }

    }
}