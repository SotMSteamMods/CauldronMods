using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TheLightAtTheEndCardController : TerminusBaseCardController
    {
        /* 
         * When this card enters play, put a one-shot from your trash into play.
         * At the end of your turn, you may remove 3 tokens from your Wrath pool. 
         * If you do, {Terminus} regains 2HP or destroys 1 ongoing card.
         */
        public TheLightAtTheEndCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override void AddTriggers()
        {
            // At the end of your turn,
            base.AddEndOfTurnTrigger((tt) => tt == base.TurnTaker, PhaseChangeActionResponse, new TriggerType[] { TriggerType.ModifyTokens, TriggerType.GainHP, TriggerType.DestroyCard });
            base.AddTriggers();
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;

            // you may remove 3 tokens from your Wrath pool
            coroutine = base.RemoveWrathTokens(3, RemoveTokensResponse, phaseChangeAction, "nothing happens.", true); 
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

        private IEnumerator RemoveTokensResponse(PhaseChangeAction phaseChangeAction, List<RemoveTokensFromPoolAction> storedResults)
        {
            IEnumerator coroutine;
            List<Function> list = new List<Function>();
            SelectFunctionDecision selectFunction;

            // If you do, {Terminus} regains 2HP or destroys 1 ongoing card.
            if (DidRemoveTokens(storedResults, 3))
            {
                list.Add(new Function(DecisionMaker, "Regain 2 HP", SelectionType.GainHP, () => base.GameController.GainHP(base.CharacterCard, 2, cardSource: base.GetCardSource())));
                list.Add(new Function(DecisionMaker, "Destroy 1 Ongoing Card", SelectionType.DestroyCard, () => base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((lcc) => IsOngoing(lcc)), false, cardSource: base.GetCardSource()))) ;
                selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, null, null, null, base.GetCardSource());

                coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = DoNothing();
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // When this card enters play, put a one-shot from your trash into play.
            coroutine = base.GameController.SelectAndMoveCard(DecisionMaker, (card) => card.Location == base.TurnTaker.Trash && card.IsInTrash && card.IsOneShot, base.TurnTaker.PlayArea, cardSource: base.GetCardSource());
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
    }
}
