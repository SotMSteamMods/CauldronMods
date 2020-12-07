﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class CricketCharacterCardController : HeroCharacterCardController
    {
        public CricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine = null;
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        List<SelectLocationDecision> storedLocation = new List<SelectLocationDecision>();
                        //Look at the top card of a deck and replace or discard it.
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, null, storedLocation, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectLocation(storedLocation))
                        {
                            coroutine = base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.TurnTakerController, storedLocation.FirstOrDefault().SelectedLocation.Location, false);
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
                case 2:
                    {
                        //Each target deals themselves 1 sonic damage.
                        coroutine = base.GameController.DealDamageToSelf(base.HeroTurnTakerController, null, 1, DamageType.Sonic, cardSource: base.GetCardSource());
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
            //Select a target. Reduce damage dealt by that target by 1 until the start of your next turn.
            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            //Select a target. 
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText)), storedResults, cardSource
                : base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.FirstOrDefault().SelectedCard != null)
            {
                //Reduce damage dealt by that target by 1 until the start of your next turn.
                ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(1);
                statusEffect.SourceCriteria.IsSpecificCard = storedResults.FirstOrDefault().SelectedCard;
                statusEffect.UntilStartOfNextTurn(base.TurnTaker);
                statusEffect.UntilTargetLeavesPlay(storedResults.FirstOrDefault().SelectedCard);
                coroutine = base.AddStatusEffect(statusEffect);
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