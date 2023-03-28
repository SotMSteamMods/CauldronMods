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
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();

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
                                                new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && tt != base.TurnTaker && !tt.IsIncapacitatedOrOutOfGame && tt.ToHero().Hand.HasCards, "other heroes with cards in hand"),
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
                coroutine = DrawACardOrPlayACard(DecisionMaker, true);
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
