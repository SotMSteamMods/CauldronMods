using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    public class AcousticDistortionCardController : CardController
    {
        public AcousticDistortionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn("OncePerTurn");
            AllowFastCoroutinesDuringPretend = false;
        }

        private const string FirstTimeWouldBeDealtDamage = "OncePerTurn";

        public override void AddTriggers()
        {
            //Once per turn when a hero target would be dealt damage, you may redirect that damage to another hero target.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn("OncePerTurn") && action.Target.IsHero, this.RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before,);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction action)
        {
            //Once per turn when a hero target would be dealt damage, you may redirect that damage to another hero target.
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetAndRedirectDamage(base.HeroTurnTakerController, (Card c) => c.IsHero && c != action.Target, action, true, storedResults, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResults.Any((SelectCardDecision d) => d.Completed && d.SelectedCard != null))
            {
                base.SetCardPropertyToTrueIfRealAction("OncePerTurn");
            }
            yield break;
        }
    }
}