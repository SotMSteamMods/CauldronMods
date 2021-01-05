using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Cauldron.Necro
{
    public abstract class UndeadCardController : NecroCardController
    {
        protected UndeadCardController(Card card, TurnTakerController turnTakerController, int baseHP) : base(card, turnTakerController)
        {
            this.BaseHP = baseHP;

            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsRitual(c) && c.IsInPlayAndHasGameText, "ritual"), null, new[] { TurnTaker });
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine = base.GameController.ChangeMaximumHP(base.Card, BaseHP + GetNumberOfRitualsInPlay(), true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            coroutine = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected int BaseHP { get; }
    }
}
