using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class MagicBladeCardController : DungeonsOfTerrorUtilityCardController
    {
        public MagicBladeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasUsedPowerThisTurn, () => $"{GetCardThisCardIsNextTo().Title} has used a power this turn.", () => $"{GetCardThisCardIsNextTo().Title} has not used a power this turn.").Condition = () => GetCardThisCardIsNextTo() != null;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "active hero"), storedResults, isPutIntoPlay, decisionSources);
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

        public override void AddTriggers()
        {
            //The first time they use a power each turn, discard and check the top card of the environment deck.",
            //If it is a fate card, that hero deals 1 target 2 energy damage. If it is not a fate card, that hero deals themselves 2 infernal damage."
            AddTrigger((UsePowerAction upa) => GetCardThisCardIsNextTo() != null && upa.HeroUsingPower == httc && !HasUsedPowerThisTurn, FirstTimePowerResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator FirstTimePowerResponse(UsePowerAction upa)
        {
            //check the top card of the environment deck.
            List<MoveCardAction> storedDiscard = new List<MoveCardAction>();
            IEnumerator coroutine = DiscardCardsFromTopOfDeck(FindEnvironment(Card.BattleZone), 1, storedResults: storedDiscard, showMessage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card cardToCheck;
            if (DidMoveCard(storedDiscard))
            {
                cardToCheck = storedDiscard.FirstOrDefault().CardToMove;
            }
            else
            {
                cardToCheck = TurnTaker.Deck.TopCard;
            }
            List<int> storedResults = new List<int>();
            coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults, TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator message = DoNothing();
            IEnumerator effect = DoNothing();
            if (storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, that hero deals 1 target 2 energy damage.
                message = GameController.SendMessageAction($"The top card of the environment deck is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = GameController.SelectTargetsAndDealDamage(httc, new DamageSource(GameController, GetCardThisCardIsNextTo()), 2, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
            }
            else if (storedResults.Any() && storedResults.First() == 0)
            {
                //If it is not a fate card, that hero deals themselves 2 infernal damage."
                message = GameController.SendMessageAction($"The top card of the environment deck is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = DealDamage(GetCardThisCardIsNextTo(), GetCardThisCardIsNextTo(), 2, DamageType.Infernal, cardSource: GetCardSource());
            }
            else
            {
                message = GameController.SendMessageAction("There are no cards in the environment deck!", Priority.High, GetCardSource(), showCardSource: true);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
            }
            yield break;
        }

        private bool HasUsedPowerThisTurn
        {
            get
            {
                return (from e in Game.Journal.UsePowerEntriesThisTurn()
                        where e.PowerUser == GetCardThisCardIsNextTo().Owner
                        select e).Count() > 1;
            }
        }

        private HeroTurnTakerController httc
        {
            get
            {
                return FindHeroTurnTakerController(GetCardThisCardIsNextTo().Owner.ToHero());
            }
        }
    }
}
