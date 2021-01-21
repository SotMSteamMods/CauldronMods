using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class GrimHeraldCardController : GargoyleUtilityCardController
    {
        //"{Gargoyle} deals 1 target 3 toxic damage.",
        //"One other player may discard a card. If they do, you may play a card or draw a card now."
        public GrimHeraldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
            IEnumerable<Function> functionChoices;
            SelectFunctionDecision selectFunction;

            //"{Gargoyle} deals 1 target 3 toxic damage."
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Toxic, 1, false, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            // One other player may discard a card,
            var selectTurnTakers = new SelectTurnTakersDecision(GameController, DecisionMaker,
                                                new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && tt != base.TurnTaker && !tt.IsIncapacitatedOrOutOfGame),
                                                SelectionType.DiscardCard, numberOfTurnTakers: 1,isOptional: false, requiredDecisions: 0, cardSource: GetCardSource());
            coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakers, (TurnTaker tt) => base.SelectAndDiscardCards(FindHeroTurnTakerController(tt.ToHero()), 1, false, 0, storedResults: storedResultsDiscard), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If they do, you may play a card or draw a card now.
            if (base.DidDiscardCards(storedResultsDiscard))
            {
                functionChoices = new Function[]
                {
				    //Play a card...
				    new Function(base.HeroTurnTakerController, "Play a card", SelectionType.PlayCard, () => base.GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardSource: base.GetCardSource())),

				    //...or draw a card.
				    new Function(base.HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => base.DrawCard())
                };
                selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, functionChoices, false);
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

            yield break;
        }
    }
}
