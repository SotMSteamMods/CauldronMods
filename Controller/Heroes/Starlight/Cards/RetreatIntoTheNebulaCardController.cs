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
            IEnumerator moveToRightPlace;
            //do we need to pick one of our multi-character promo to protect?
            if (IsMultiCharPromo(allowReplacements: false))
            {

                //The 'move-to-starlight' text is on the instruction card - 
                //I expect it is very unlikely to need card/turntakercontroller replacements!
                moveToRightPlace = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && ListStarlights(allowReplacements: false).Contains(c), "Starlight"), storedResults, isPutIntoPlay, decisionSources);

            }
            else
            {
                //let the default handle it if not
                moveToRightPlace = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(moveToRightPlace);
            }
            else
            {
                GameController.ExhaustCoroutine(moveToRightPlace);
            }

            yield break;
        }

        private bool IsProtectedCard(Card c)
        {
            bool shouldProtect = false;

            if (GetCardThisCardIsNextTo(allowReplacements: false) != null)
            {
                //proper logic:
                //if EITHER it is next to the character card being damaged
                //      (because it was put there when we played it)
                bool isNextToTarget = c == GetCardThisCardIsNextTo(false);

                //OR it is not next to any character card owned by the same player - even incapacitated ones
                //      (and therefore it is being borrowed by someone else like Guise)
                bool isNotBeingBorrowed = ListStarlights().Contains(GetCardThisCardIsNextTo(false));

                //THEN prevent the damage
                shouldProtect = isNextToTarget && isNotBeingBorrowed;
            }
            else
            {
                shouldProtect = ListStarlights().Contains(c);

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