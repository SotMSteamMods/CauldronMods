using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfLightningCardController : SpellCardController
    {
        public ElementOfLightningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroWithMostCards(true);
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfLightning"));
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            Card characterCard = base.TurnTaker.FindCard("StormTiamatCharacter");
            //If {Tiamat}, The Eye of the Storm is active, she deals each hero target 2+X lightning damage, where X is the number of Element of Lightning cards in the villain trash.
            if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
            {
                Func<Card, int?> X = (Card c) => new int?(PlusNumberOfThisCardInTrash(2));
                coroutine = base.DealDamage(characterCard, (Card c) => c.IsHero, X, DamageType.Lightning);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The hero with the most cards in hand...
            List<TurnTaker> storedResults = new List<TurnTaker>();
            coroutine = base.FindHeroWithMostCardsInHand(storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Count<TurnTaker>() > 0)
            {
                //...may not draw cards until the start of the next villain turn.
                TurnTaker isSpecificTurnTaker = storedResults.First<TurnTaker>();
                PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = new Phase?(Phase.DrawCard);
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = isSpecificTurnTaker;
                preventPhaseActionStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                coroutine = base.AddStatusEffect(preventPhaseActionStatusEffect);
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