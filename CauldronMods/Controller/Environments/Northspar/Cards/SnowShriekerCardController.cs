using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cauldron.Northspar
{
    public class SnowShriekerCardController : NorthsparCardController
    {

        public SnowShriekerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(ranking: 2);
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsFrozen(c), "frozen"));
        }

        public override void AddTriggers()
        {
            //Increase fire damage dealt to this card by 2.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Fire && dd.Target == base.Card, (DealDamageAction dd) => 2);

            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP X cold damage, where X = 1 plus the number of Frozen cards in the environment trash.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //this card deals the non-environment target with the second lowest HP X cold damage, where X = 1 plus the number of Frozen cards in the environment trash
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 2, (Card c) => c.IsNonEnvironmentTarget, (Card c) => GetNumberOfFrozensInTrash + 1, DamageType.Cold);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private int GetNumberOfFrozensInTrash
        {
            get
            {
                int result = FindCardsWhere((Card c) => IsFrozen(c) && base.TurnTaker.Trash.Cards.Contains(c)).Count();
                return result;           
            }
        }

    }
}