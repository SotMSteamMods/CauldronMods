using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class MalichaeCharacterCardController : HeroCharacterCardController
    {
        public MalichaeCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //PowerNumerals required on powers
            int cardsToDraw = GetPowerNumeral(0, 2);
            var coroutine = base.DrawCards(this.DecisionMaker, cardsToDraw);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.SelectAndDiscardCards(this.DecisionMaker, 1);
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
                        List<SelectTurnTakerDecision> storedResultsTurnTaker = new List<SelectTurnTakerDecision>();
                        var coroutine = base.GameController.SelectHeroToDiscardCard(this.DecisionMaker,
                            storedResultsTurnTaker: storedResultsTurnTaker,
                            storedResultsDiscard: storedResultsDiscard,
                            cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidDiscardCards(storedResultsDiscard, 1))
                        {
                            var httc = base.FindHeroTurnTakerController(GetSelectedTurnTaker(storedResultsTurnTaker).ToHero());
                            coroutine = base.DrawCards(httc, 3);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        var coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment, "enviroment"), false, cardSource: GetCardSource());
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
                case 2:
                    {
                        List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
                        var coroutine = base.GameController.SelectDamageType(this.DecisionMaker, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        var selectedType = GetSelectedDamageType(storedResults);
                        if (selectedType.HasValue)
                        {
                            ChangeDamageTypeStatusEffect status = new ChangeDamageTypeStatusEffect(selectedType.Value);
                            status.SourceCriteria.IsHero = true;
                            status.SourceCriteria.IsTarget = true;
                            status.UntilStartOfNextTurn(this.TurnTaker);
                            status.CardSource = Card;

                            coroutine = base.AddStatusEffect(status);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
