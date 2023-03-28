using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class MesmerPendantCardController : CardController
    {
        public MesmerPendantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card currently does not work.");
            AddThisCardControllerToList(CardControllerListType.ModifiesDeckKind);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a non-character target.",
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter), storedResults, isPutIntoPlay, decisionSources);
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
            //"Treat the words 'Hero Target', 'Hero Ongoing', and 'Non-Villain target' on [the card this is next to] as 'Villain Target', 'Villain Ongoing' and 'Non-Hero target' instead.",
            AddTrigger((GameAction ga) => ga.CardSource != null && ga.CardSource.Card == GetCardThisCardIsNextTo(), LogAction, TriggerType.Hidden, TriggerTiming.Before);
            //"At the start of your turn destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
            
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: false);

        }
        private IEnumerator LogAction(GameAction ga)
        {
            //Log.Debug($"GameAction detected from {ga.CardSource.Card.Title}");
            yield break;
        }

        //i think the best we can do is turn "non-villain" into "non-hero"
        public override bool? AskIfIsVillain(Card card, CardSource cardSource)
        {
            Log.Debug("Mesmer pendant's ask-if-is-villain fires");
            if (cardSource != null && cardSource.Card == GetCardThisCardIsNextTo())
            {
                if(card.IsTarget || IsOngoing(card))
                {
                    if(card.IsVillain)
                    {
                        return false;
                    }
                    if(IsHero(card))
                    {
                        return true;
                    }
                }
            }
            return null;
        }

        public override bool? AskIfIsVillainTarget(Card card, CardSource cardSource)
        {
            Log.Debug("Mesmer Pendant's ask-if-is-villain-target fires");
            //it doesn't seem to
            if (cardSource != null && cardSource.Card == GetCardThisCardIsNextTo())
            {
                if(card.IsTarget && IsHero(card))
                {
                    return true;
                }
                if(card.IsTarget && IsVillain(card))
                {
                    return false;
                }
            }
            return null;
        }
    }
}