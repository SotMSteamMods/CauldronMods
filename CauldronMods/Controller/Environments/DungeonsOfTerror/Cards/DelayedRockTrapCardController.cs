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
            //When a card enters the environment trash, check that card. If it is a fate card, this card deal the hero next to it {H} melee damage. If it is not a fate card, this card deals each other hero target {H-2} melee damage. Then, destroy this card.
            AddTrigger((MoveCardAction mca) => mca.Destination == TurnTaker.Trash && GetCardThisCardIsNextTo() != null, CardEntersTrashResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
            AddTrigger((BulkMoveCardsAction bmca) => bmca.Destination == TurnTaker.Trash && GetCardThisCardIsNextTo() != null, CardEntersTrashResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
        }

        private IEnumerator CardEntersTrashResponse(BulkMoveCardsAction bmca)
        {
            IEnumerator coroutine;
            foreach(Card card in bmca.CardsToMove)
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
            Card card = mca.CardToMove;
            IEnumerator coroutine = CheckForFateAndDealDamage(card, mca);
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

        private IEnumerator CheckForFateAndDealDamage(Card card, GameAction triggeringAction)
        {
            //check that card.
            List<int> storedResults = new List<int>();
            IEnumerator coroutine = CheckForNumberOfFates(card.ToEnumerable(), storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, this card deal the hero next to it {H} melee damage.
                coroutine = GameController.SendMessageAction($"{Card.Title} was a fate card!", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DealDamage(Card, GetCardThisCardIsNextTo(), Game.H, DamageType.Melee, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else
            {
                //If it is not a fate card, this card deals each other hero target {H-2} melee damage. 
                coroutine = GameController.SendMessageAction($"{Card.Title} was not a fate card!", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DealDamage(Card, (Card c) => c.IsHero && c.IsTarget && c != GetCardThisCardIsNextTo(), Game.H - 2, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(triggeringAction);
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
