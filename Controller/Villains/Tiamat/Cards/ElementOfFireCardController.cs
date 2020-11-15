using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfFireCardController : SpellCardController
    {
        public ElementOfFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroWithMostCards(false);
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfFire"));
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            Card characterCard = base.TurnTaker.FindCard("InfernoTiamatCharacter");
            //If {Tiamat}, The Mouth of the Inferno is active, she deals each hero target 2+X fire damage, where X is the number of Element of Fire cards in the villain trash.
            if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
            {
                Func<Card, int?> X = (Card c) => new int?(PlusNumberOfThisCardInTrash(2));
                coroutine = base.DealDamage(characterCard, (Card c) => c.IsHero, X, DamageType.Fire);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The hero with the most cards in play...
            List<TurnTaker> storedResults = new List<TurnTaker>();
            coroutine = base.FindHeroWithMostCardsInPlay(storedResults);
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
                //...may not play cards until the start of the next villain turn.
                TurnTaker isSpecificTurnTaker = storedResults.First<TurnTaker>();
                CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
                cannotPlayCardsStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = isSpecificTurnTaker;
                cannotPlayCardsStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                coroutine = base.AddStatusEffect(cannotPlayCardsStatusEffect);
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