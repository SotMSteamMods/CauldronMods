using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class ChampionOfTheRealmCardController : TheKnightCardController
    {
        public ChampionOfTheRealmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(this.TurnTaker.Deck, new LinqCardCriteria(c => IsEquipment(c) && IsSingleHandCard(c), "single hand equipment"));
        }

        public override IEnumerator Play()
        {
            //"Search your deck for a Single Hand Equipment card and put it into play. Shuffle your deck.",
            //"Select a hero target. Until the start of your next turn, increase damage dealt by that target by 1."
            var criteria = new LinqCardCriteria(c => IsEquipment(c) && IsSingleHandCard(c), "single hand equipment");
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
            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.IncreaseDamage, criteria, storedResult, false, cardSource: base.GetCardSource());
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
                IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
                increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                increaseDamageStatusEffect.UntilTargetLeavesPlay(card);

                coroutine = base.AddStatusEffect(increaseDamageStatusEffect, true);
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
