using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class DefenderOfTheRealmCardController : TheKnightCardController
    {
        public DefenderOfTheRealmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => c.Identifier == "PlateHelm" || c.Identifier == "PlateMail", "armor"));
        }

        public override IEnumerator Play()
        {
            //"Search your deck for a copy of “Plate Mail” or “Plate Helm” and put it into play. Shuffle your deck.",
            //"Select a hero target. Until the start of your next turn, reduce damage dealt to that target by 1."
            var criteria = new LinqCardCriteria(c => c.Identifier == "PlateHelm" || c.Identifier == "PlateMail", "armor");
            var coroutine = base.SearchForCards(this.DecisionMaker, true, false, 1, 1, criteria, true, false, false, shuffleAfterwards: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            var storedResult = new List<SelectCardDecision>();
            criteria = new LinqCardCriteria(c => IsHeroTarget(c) && c.IsInPlayAndHasGameText, "hero target");
            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.ReduceDamageTaken, criteria, storedResult, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(storedResult))
            {
                Card card = GetSelectedCard(storedResult);
                ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = card;
                reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                reduceDamageStatusEffect.UntilTargetLeavesPlay(card);

                coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
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
