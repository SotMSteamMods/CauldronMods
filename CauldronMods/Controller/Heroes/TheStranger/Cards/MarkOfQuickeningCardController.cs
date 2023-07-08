using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfQuickeningCardController : RuneCardController
    {
        #region Constructors

        public MarkOfQuickeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
        }

        #endregion Constructors

        #region Properties
        public override LinqCardCriteria NextToCardCriteria => new LinqCardCriteria((Card c) => IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame, "hero", useCardsSuffix: true);
        #endregion

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //They may play an additional card during their play phase.
            base.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.PlayCard, 1);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo(true)?.Owner, Phase.PlayCard, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return base.GetCardThisCardIsNextTo(true) != null && tt == base.GetCardThisCardIsNextTo(true).Owner;
        }
        #endregion Methods
    }
}