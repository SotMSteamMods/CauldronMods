using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class RetreatIntoTheNebulaCardController : StarlightCardController
    {
        public RetreatIntoTheNebulaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to {Starlight} by 2.",
            AddReduceDamageTrigger(IsProtectedCard, 2);
            //"At the start of your turn, destroy a constellation card or destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyThisOrConstellation, TriggerType.DestroyCard);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine;
            //do we need to pick one of our multi-character promo to protect?
            if (IsMultiCharPromo())
            {
                //TODO - Retreat's find-character-to-play-next-to multichar logic
                coroutine = GameController.SendMessageAction("Thinks we're in MultiCharPromo", Priority.Low, GetCardSource());

            }
            else
            {
                //let the default handle it if not
                coroutine = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private bool IsProtectedCard(Card c)
        {
            bool shouldProtect = false;

            if (IsMultiCharPromo())
            {
                //TODO - Retreat's multichar protection logic
                return false;
                //proper logic:
                //if EITHER it is next to the character card being damaged
                //      (because it was put there when we played it)
                //OR it is not next to any of them - even incapacitated ones
                //      (and therefore it is being borrowed by someone else)
                //THEN prevent the damage
            }
            else
            {
                Card ownCharCard = ListStarlights().FirstOrDefault();
                shouldProtect = c == ownCharCard;

            }

            return c.IsTarget && shouldProtect;
        }

        private IEnumerator DestroyThisOrConstellation(PhaseChangeAction pc)
        {
            var criteria = new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && (IsConstellation(c) || base.Card == c));
            IEnumerator coroutine = GameController.SelectAndDestroyCard(HeroTurnTakerController, criteria, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}