using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class StoneWardenCardController : DungeonsOfTerrorUtilityCardController
    {
        public StoneWardenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildTopCardOfLocationSpecialString(TurnTaker.Trash));
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => IsTopCardOfLocationFate(TurnTaker.Trash) == true;
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP().Condition = () => IsTopCardOfLocationFate(TurnTaker.Trash) == false;
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, check the top card of the environment trash.
            //If it is a fate card, this card deals the hero target with the highest HP {H} melee damage. If it is not a fate card, this card deals the non-environment target with the lowest HP 2 melee damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, CheckForFatesResponse, TriggerType.DealDamage);

        }

        private IEnumerator CheckForFatesResponse(PhaseChangeAction pca)
        {
            //check the top card of the environment trash.
            List<int> storedResults = new List<int>();
            Card cardToCheck = TurnTaker.Trash.TopCard;
            List<bool> suppressMessage = new List<bool>();
            IEnumerator coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults, TurnTaker.Trash, suppressMessage);
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
                //If it is a fate card, this card deals the hero target with the highest HP {H} melee damage.
                message = GameController.SendMessageAction($"The top card of the environment trash is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = DealDamageToHighestHP(Card, 1, (Card c) => IsHeroTarget(c), (Card c) => Game.H, DamageType.Melee);
            }
            else if (storedResults.Any() && storedResults.First() == 0)
            {
                //If it is not a fate card, this card deals the non-environment target with the lowest HP 2 melee damage.
                if (suppressMessage.Any() && suppressMessage.First() != true)
                {
                    message = GameController.SendMessageAction($"The top card of the environment trash is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                }
                effect = DealDamageToLowestHP(Card, 1,  (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()),(Card c) => 2, DamageType.Melee);
            }
            else
            {
                message = GameController.SendMessageAction("There are no cards in the environment trash!", Priority.High, GetCardSource(), showCardSource: true);
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
    }
}
