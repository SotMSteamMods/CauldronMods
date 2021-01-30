﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class AllInGoodTimeDriftCharacterCardController : DriftSubCharacterCardController
    {
        public AllInGoodTimeDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard cards from the top of your deck until you discard an ongoing. Play or draw it.
            List<RevealCardsAction> revealCards = new List<RevealCardsAction>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, (Card c) => c.IsOngoing, 1, revealCards, RevealedCardDisplay.ShowMatchingCards, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.MoveCards(base.TurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location.IsRevealed)), base.TurnTaker.Trash, isDiscard: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card matchingCard = revealCards.FirstOrDefault().RevealedCards.LastOrDefault();
            coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[]
            {
                new Function(base.HeroTurnTakerController, "Play revealed card", SelectionType.PlayCard, () => base.GameController.PlayCard(base.TurnTakerController, matchingCard, cardSource: base.GetCardSource())),
                new Function(base.HeroTurnTakerController, "Draw revealed card", SelectionType.DrawCard, () => base.GameController.MoveCard(base.TurnTakerController, matchingCard,base.TurnTaker.ToHero().Hand, cardSource: base.GetCardSource()))
            });
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
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One hero may play a card now.
                        coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitated));
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
                        //Destroy 1 ongoing card.
                        coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing), false, cardSource: GetCardSource());
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
                case 2:
                    {
                        //Select a target. Increase the next damage it deals by 2.
                        List<SelectCardDecision> cardDecision = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.IncreaseNextDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), cardDecision, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        Card selectedTarget = cardDecision.FirstOrDefault().SelectedCard;
                        IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(2);
                        statusEffect.NumberOfUses = 1;
                        statusEffect.SourceCriteria.IsSpecificCard = selectedTarget;
                        statusEffect.UntilCardLeavesPlay(selectedTarget);

                        coroutine = base.AddStatusEffect(statusEffect);
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
    }
}
