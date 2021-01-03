using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public abstract class TacticBaseCardController : CardController
    {
        protected bool CanExtend = true;
        protected bool DrawWhenDropping = true;
        protected TriggerType[] ResponseTriggerTypes
        {
            get
            {
                var effects = new List<TriggerType> { TriggerType.DestroySelf };
                if(CanExtend)
                {
                    effects.Add(TriggerType.DiscardCard);
                    if(DrawWhenDropping)
                    {
                        effects.Add(TriggerType.DrawCard);
                    }
                }
                return effects.ToArray();
            }
        }
        public TacticBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnTacticResponse, ResponseTriggerTypes);
            AddTacticEffectTrigger();
        }

        protected abstract void AddTacticEffectTrigger();

        private IEnumerator StartOfTurnTacticResponse(PhaseChangeAction pc)
        {
            var discardStorage = new List<DiscardCardAction>();
            IEnumerator coroutine;

            //"At the start of your turn... 
            
            //[sometimes]: ...you may discard a card. 
            if(CanExtend)
            {
                coroutine = SelectAndDiscardCards(DecisionMaker, 1, false, 0, discardStorage);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //[extensibles: If you do not, [sometimes: draw a card and]] destroy this card.",
            if(!DidDiscardCards(discardStorage))
            {
                if(CanExtend && DrawWhenDropping)
                {
                    coroutine = DrawCard();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                coroutine = DestroyThisCardResponse(pc);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}