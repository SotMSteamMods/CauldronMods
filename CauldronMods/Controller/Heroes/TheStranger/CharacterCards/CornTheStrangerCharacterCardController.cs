using Handelabra.Sentinels.Engine.Controller;
using Handelabra;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class CornTheStrangerCharacterCardController : HeroCharacterCardController
    {
        public CornTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => IsRuneNextToTarget(c), "targets next to a rune"));
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //{TheStranger} or a target next to a Rune deals 1 target 2 psychic damage.
            int target = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 2);

            IEnumerable<Card> sourceChoices = base.Card.ToEnumerable();
            IEnumerable<Card> targetsNextToRunes = FindTargetsNotTheStrangerNextToRunes();
            sourceChoices = sourceChoices.Concat(targetsNextToRunes);
           
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => sourceChoices.Contains(c), base.Card.Title + " or a target next to a Rune"), storedResults, optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            if (DidSelectCard(storedResults))
            {
                Card source = GetSelectedCard(storedResults);
                coroutine2 = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, source), amount, DamageType.Psychic, target, optional: false, target, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //One hero may use a power now.
                        IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;

                    }
                case 2:
                    {
                        //One target deals itself 1 irreducible toxic damage.
                        List<SelectCardDecision> storedResult = new List<SelectCardDecision>();
                        IEnumerator coroutine3 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DealDamageSelf, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target"), storedResult, optional: false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        if (DidSelectCard(storedResult))
                        {
                            Card selectedCard = GetSelectedCard(storedResult);
                            coroutine3 = DealDamage(selectedCard, selectedCard, 1, DamageType.Toxic, isIrreducible: true, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }

        private IEnumerable<Card> FindTargetsNotTheStrangerNextToRunes()
        {
           
            return FindCardsWhere((Card c) => c != base.Card && IsRuneNextToTarget(c));
        }

        private bool IsRuneNextToTarget(Card c)
        {
            return c.IsInPlay && c.IsTarget && c.NextToLocation != null && c.NextToLocation.Cards.Any((Card r) => IsRune(r));
        }

    }
}
