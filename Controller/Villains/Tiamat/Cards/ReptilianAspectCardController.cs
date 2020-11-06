using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ReptilianAspectCardController : CardController
    {
        #region Constructors

        public ReptilianAspectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the villain turn each Head regains {H - 2} HP.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.GainHPResponse), TriggerType.GainHP, null, false);
        }
        private IEnumerator GainHPResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine = base.GameController.GainHP(this.DecisionMaker, (Card card) => card.DoKeywordsContain("head"), base.Game.H - 2, null, false, null, null, null, base.GetCardSource(null));
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

        #endregion Methods
    }
}