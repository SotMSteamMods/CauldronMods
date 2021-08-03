using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Malichae
{
    public abstract class DjinnOngoingController : MalichaeCardController
    {
        private readonly string _requiredIdentifier;
        private readonly string _attachIdentifier;

        protected DjinnOngoingController(Card card, TurnTakerController turnTakerController, string requiredIdentifier, string attachIdentifier)
            : base(card, turnTakerController)
        {
            _requiredIdentifier = requiredIdentifier;
            _attachIdentifier = attachIdentifier;
        }

        //This is here for possible Promo support.
        public abstract Power GetGrantedPower(CardController cardController, Card damageSource=null);

        protected CardSource GetCardSourceForGrantedPower()
        {
            CardSource cs;
            var card = GetCardThisCardIsNextTo();
            if (card is null)
            {
                cs =  GetCardSource().AssociatedCardSources.FirstOrDefault();
            }
            else
            {
                var cc = FindCardController(card);
                cs =  cc.GetCardSource();
            }

            return cs;
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.Card == base.GetCardThisCardIsNextTo(true))
            {
                return new Power[]
                {
                    GetGrantedPower(cardController),
                };
            }
            return null;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Place this card next to a target
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && c.Identifier == _attachIdentifier, _attachIdentifier), storedResults, isPutIntoPlay, decisionSources);
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
            AddTrigger<MoveCardAction>(MoveCriteria, RequiredCardMissingDestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After);
            AddTrigger<DestroyCardAction>(DestroyCriteria, RequiredCardMissingDestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After);
            base.AddAsPowerContributor();
        }

        protected void AddDestroyAtEndOfTurnTrigger()
        {
            AddEndOfTurnTrigger(tt => tt == this.TurnTaker, pca => DestroySelf(pca), TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            return CheckPlayConditionsAreMet();
        }

        protected IEnumerator CheckPlayConditionsAreMet()
        {
            var query = GameController.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == _requiredIdentifier, visibleToCard: GetCardSource());
            if (!query.Any())
            {
                var coroutine = RequiredCardMissingDestroySelfResponse(null);
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

        private bool DestroyCriteria(DestroyCardAction dca)
        {
            //this should only apply to the GrandOngoings that need High

            //your are my requiredIdentifer
            //me and you are both in play
            //you are moving to the hand or trash
            return (dca.CardToDestroy.Card.Identifier == _requiredIdentifier || dca.CardToDestroy.Card.Identifier == _attachIdentifier) &&
                   dca.WasCardDestroyed &&
                   dca.CardToDestroy.Card.Location.IsHeroPlayAreaRecursive;
        }

        private bool MoveCriteria(MoveCardAction mca)
        {
            //your are my requiredIdentifer
            //me and you are both in play
            //you are moving to the hand or trash
            return (mca.CardToMove.Identifier == _requiredIdentifier || mca.CardToMove.Identifier == _attachIdentifier) &&
                   mca.Origin.IsHeroPlayAreaRecursive &&
                   (mca.Destination.IsHand || mca.Destination.IsTrash);
        }

        private IEnumerator RequiredCardMissingDestroySelfResponse(GameAction ga)
        {
            var sample = FindCard(_requiredIdentifier);
            string message = $"{sample.Title} is not in play, {Card.Title} will be destroyed.";
            IEnumerator coroutine = GameController.SendMessageAction(message, Priority.Medium, cardSource: GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DestroySelf(ga);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected IEnumerator DestroySelf(GameAction ga = null)
        {
            return base.GameController.DestroyCard(this.DecisionMaker, Card,
                actionSource: ga,
                cardSource: GetCardSource());
        }

        public IEnumerable<Card> FindBaseDjinn()
        {
            return FindCardsWhere(c => c.Identifier == _attachIdentifier);
        }

        private bool IsBaseDjinnInPlay()
        {
            return FindBaseDjinn().Where(c => c.IsInPlayAndHasGameText).Any();
        }
    }
}
