using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class SmokeAndMirrorsCardController : CardController
    {
        public SmokeAndMirrorsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //"When a hero target would be dealt damage, you may prevent that damage. If you do, destroy this card.",
            AddTrigger((DealDamageAction dd) => IsHero(dd.Target) && dd.Amount > 0 && !this.IsBeingDestroyed, PreventDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isActionOptional: true);
        }

        private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
            var yesNoStorage = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.PreventDamage, this.Card, dd, yesNoStorage, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(yesNoStorage))
            {
                coroutine = CancelAction(dd, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (IsRealAction(dd))
                {
                    coroutine = GameController.DestroyCard(DecisionMaker, this.Card, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }
    }
}