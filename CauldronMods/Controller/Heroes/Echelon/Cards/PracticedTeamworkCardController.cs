using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class PracticedTeamworkCardController : TacticBaseCardController
    {
        //==============================================================
        //"When this card enters play, each player may draw a card.",
        //"At the start of your turn, you may discard a card. If you do not, destroy this card.",
        //"Reduce damage dealt to hero targets by hero targets by 1."
        //==============================================================

        public static string Identifier = "PracticedTeamwork";

        public PracticedTeamworkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.DrawWhenDropping = false;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = EachPlayerDrawsACard(optional: true);
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

        protected override void AddTacticEffectTrigger()
        {
            //"Reduce damage dealt to hero targets by hero targets by 1."
            AddReduceDamageTrigger((DealDamageAction dd) => IsHero(dd.Target) && dd.DamageSource.IsHero && dd.DamageSource.IsTarget, _ => 1);
        }
    }
}