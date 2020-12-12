using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Cricket
{
    public class SilentStalkerCardController : CardController
    {
        public SilentStalkerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.DidDealDamageThisTurn(), () => "Cricket has dealt damage this turn.", () => "Cricket has not dealt damage this turn.");
        }

        //At the end of your turn, if {Cricket} dealt no damage this turn, you may use a power.
        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.UsePowerResponse, TriggerType.UsePower, (PhaseChangeAction action) => this.DidDealDamageThisTurn());
        }

        private IEnumerator UsePowerResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: GetCardSource());
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

        private bool DidDealDamageThisTurn()
        {
            return !base.Journal.DealDamageEntriesThisTurnSinceCardWasPlayed(base.CharacterCard).Any();
        }
    }
}