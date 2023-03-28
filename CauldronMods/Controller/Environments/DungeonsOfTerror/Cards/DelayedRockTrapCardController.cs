using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class DelayedRockTrapCardController : DungeonsOfTerrorUtilityCardController
    {
        public DelayedRockTrapCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "active hero"), storedResults, isPutIntoPlay, decisionSources);
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
            //When a card enters the environment trash, check that card. If it is a fate card, this card deal the hero next to it {H} melee damage. If it is not a fate card, this card deals each other hero target {H-2} melee damage. Then, destroy this card.
            AddTrigger((MoveCardAction mca) => mca.Destination == FindEnvironment(Card.BattleZone).TurnTaker.Trash && GetCardThisCardIsNextTo() != null, CardEntersTrashResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
            AddTrigger((BulkMoveCardsAction bmca) => bmca.Destination == FindEnvironment(Card.BattleZone).TurnTaker.Trash && GetCardThisCardIsNextTo() != null, CardEntersTrashResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
        }

        private IEnumerator CardEntersTrashResponse(BulkMoveCardsAction bmca)
        {
            IEnumerator coroutine = GameController.SendMessageAction("A card has entered the environment trash!", Priority.Medium, GetCardSource(), showCardSource: true);
            if(base.UseUnityCoroutines)
                {
                yield return base.GameController.StartCoroutine(coroutine);
            }
                else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            foreach (Card card in bmca.CardsToMove)
            {
                coroutine = CheckForFateAndDealDamage(card, bmca);
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

        private IEnumerator CardEntersTrashResponse(MoveCardAction mca)
        {
            IEnumerator coroutine = GameController.SendMessageAction("A card has entered the environment trash!", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = mca.CardToMove;
            coroutine = CheckForFateAndDealDamage(card, mca);
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

        private IEnumerator CheckForFateAndDealDamage(Card cardToCheck, GameAction triggeringAction)
        {
            //check that card.
            List<int> storedResults = new List<int>();
            IEnumerator coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults);
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
            if(storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, this card deal the hero next to it {H} melee damage.
                message = GameController.SendMessageAction($"{cardToCheck.Title} is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = DealDamage(Card, GetCardThisCardIsNextTo(), Game.H, DamageType.Melee, cardSource: GetCardSource());
            }
            else if(storedResults.Any() && storedResults.First() == 0)
            {
                //If it is not a fate card, this card deals each other hero target {H-2} melee damage. 
                message = GameController.SendMessageAction($"{cardToCheck.Title} is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = DealDamage(Card, (Card c) => IsHeroTarget(c) && c != GetCardThisCardIsNextTo(), Game.H - 2, DamageType.Melee);
            }

            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(triggeringAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
