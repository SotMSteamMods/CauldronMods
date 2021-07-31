using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class AethiumCannonCardController : NightloreCitadelUtilityCardController
    {
        public AethiumCannonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
        }


        public static readonly string Firing = "Firing";

        private bool CanConnonFire
        {
            get
            {
                bool? firing = GetCardPropertyJournalEntryBoolean(Firing);
                if (firing != null && firing.Value == true)
                {
                    return false;
                }
                return true;
            }
        }
        public override void AddTriggers()
        {
            // At the end of the environment turn, each player may put 1 card from their hand beneath this one. Cards beneath this one are not considered in play.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, MoveCardsResponse, TriggerType.MoveCard);
            AddBeforeLeavesPlayActions(MoveCardsUnderThisCardToTrash);
            // If there are ever {H} times 3 cards beneath this one, this card deals 1 target 15 radiant damage and those cards are discarded.
            AddTrigger((GameAction action) => CanConnonFire && Card.UnderLocation.NumberOfCards >= Game.H * 3, FireCannonResponse, new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.MoveCard
                }, TriggerTiming.After);

            //Cards under this card are not considered in play
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && !md.CardSource.Card.IsEnvironment, this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);

        }

        private IEnumerator FireCannonResponse(GameAction arg)
        {
            SetCardProperty(Firing, true);
            //this card deals 1 target 15 radiant damage
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 15, DamageType.Radiant, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //those cards are discarded
            coroutine = MoveCardsUnderThisCardToTrash();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SetCardProperty(Firing, false);
            yield break;
        }

        private IEnumerator MoveCardsUnderThisCardToTrash(GameAction gameAction)
        {
            IEnumerator coroutine = MoveCardsUnderThisCardToTrash();
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

        private IEnumerator MoveCardsUnderThisCardToTrash()
        {
            IEnumerator coroutine = base.GameController.MoveCards(TurnTakerController, base.Card.UnderLocation.Cards, (Card c) => new MoveCardDestination(c.Owner.Trash), cardSource: GetCardSource());
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

        private IEnumerator MoveCardsResponse(PhaseChangeAction phaseChange)
        {
            //each player may put 1 card from their hand beneath this one.
            IEnumerator coroutine = EachPlayerMovesCards(0, 1, SelectionType.MoveCardToUnderCard, new LinqCardCriteria((Card c) => true),
                (HeroTurnTaker htt) => htt.Hand, (HeroTurnTaker htt) => new List<MoveCardDestination>() { new MoveCardDestination(Card.UnderLocation) },
                requiredNumberOfHeroes: 0, playIfMovingToPlayArea: false, associatedCards: Card.ToEnumerable());

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

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => Card.UnderLocation.HasCard(d.SelectedCard));
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if (Card.UnderLocation.HasCard(card) && !cardSource.Card.IsEnvironment)
            {
                return false;
            }
            return true;
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {

            bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => !c.IsEnvironment, (Card c) => Card.UnderLocation.HasCard(c));

            if (flag != null && flag.Value)
            {
                return false;
            }

            return true;
        }
    }
}
