using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class CrumblingRuinsCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public CrumblingRuinsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsEnvironmentTarget, "", useCardsSuffix: false, singular: "environment target", plural: "environment targets"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each non-environment target X melee damage, where X is the number of environment targets in play.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = DealDamage(Card, (Card c) => c.IsNonEnvironmentTarget, (Card c) => GetNumberOfEnvironmentTargetsInPlay, DamageType.Melee);
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

        private int GetNumberOfEnvironmentTargetsInPlay
        {
            get
            {
                return FindCardsWhere(c => c.IsEnvironmentTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Count();
            }
        }
    }
}
