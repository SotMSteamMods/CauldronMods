﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Haka;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfIceCardController : SpellCardController
    {
        public ElementOfIceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfIce"));
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            Card characterCard = base.TurnTaker.FindCard("WinterTiamatCharacter");
            //If {Tiamat}, The Jaws of Winter is active, she deals each hero target 2+X cold damage, where X is the number of Element of Ice cards in the villain trash.
            if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
            {
                Func<Card, int?> X = (Card c) => new int?(PlusNumberOfThisCardInTrash(2));
                coroutine = base.DealDamage(characterCard, (Card c) => c.IsHero, X, DamageType.Cold);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The hero with the highest HP...
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            LinqCardCriteria criteria = new LinqCardCriteria((Card card) => base.CanCardBeConsideredHighestHitPoints(card, (Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsFlipped));
            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.HeroCharacterCard, criteria, storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card highestHpHero = highestHpHero = storedResults.FirstOrDefault().SelectedCard;

            //...may not use powers until the start of the next villain turn.
            CannotUsePowersStatusEffect cannotUsePowersStatusEffect = new CannotUsePowersStatusEffect();
            cannotUsePowersStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = highestHpHero.NativeDeck.OwnerTurnTaker;
            cannotUsePowersStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            coroutine = base.AddStatusEffect(cannotUsePowersStatusEffect);
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