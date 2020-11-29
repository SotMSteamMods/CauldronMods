using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    public class UncannyQuicksilverCharacterCardController : HeroCharacterCardController
    {
        public UncannyQuicksilverCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw until they have 4 cards in hand.
                        List<SelectTurnTakerDecision> selection = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(base.HeroTurnTakerController, SelectionType.DrawCard, true, false, selection, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && base.FindHeroTurnTakerController(tt.ToHero()).NumberOfCardsInHand < 4));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        coroutine = base.DrawCardsUntilHandSizeReached(base.FindHeroTurnTakerController(selection.FirstOrDefault().SelectedTurnTaker.ToHero()), 4);
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
                        //One player may use a power now.
                        IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        //One player may discard any number of cards to regain X HP, where X is the number of cards discarded.
                        List<SelectTurnTakerDecision> selection = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(base.HeroTurnTakerController, SelectionType.DrawCard, true, false, selection, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && base.FindHeroTurnTakerController(tt.ToHero()).NumberOfCardsInHand > 0));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        HeroTurnTakerController selectedHero = base.FindHeroTurnTakerController(selection.FirstOrDefault().SelectedTurnTaker.ToHero());
                        List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
                        coroutine = base.GameController.SelectAndDiscardCards(selectedHero, selectedHero.NumberOfCardsInHand, true, 0, storedResults, true, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        coroutine = base.GameController.GainHP(selectedHero.CharacterCard, base.GetNumberOfCardsDiscarded(storedResults) * 2, cardSource: base.GetCardSource());
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
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetNumeral = GetPowerNumeral(0, 1);
            int damageNumeral = GetPowerNumeral(1, 2);

            //{Quicksilver} deals 1 target 2 melee damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.Card), damageNumeral, DamageType.Melee, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
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