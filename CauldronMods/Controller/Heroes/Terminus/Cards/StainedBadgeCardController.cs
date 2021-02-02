using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class StainedBadgeCardController : TerminusMementoCardController
    {
        /*
         * This card and {Terminus} are indestructible unless all other heroes are incapacitated. If another Memento would 
         * enter play, instead remove it from the game.
         * At the end of your turn, add 1 token to your Wrath pool if {Terminus} has 1 or more HP.
         */
        public StainedBadgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt == base.TurnTaker && base.CharacterCard.HitPoints > 0, PhaseChangeActionResponse, TriggerType.AddTokensToPool);
            base.AddTriggers();
        }

        protected IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;

            coroutine = base.AddWrathTokens(1); 
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

        protected override IEnumerator OnOtherMementoRemoved()
        {            
            return DoNothing();
        }
    }
}
