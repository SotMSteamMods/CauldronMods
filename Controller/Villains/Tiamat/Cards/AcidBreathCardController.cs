﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class AcidBreathCardController : CardController
    {

        public AcidBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            //Destroy 2 hero ongoing cards and 2 hero equipment cards.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing && c.IsHero), new int?(2), storedResultsAction: storedResultsAction, cardSource: this.GetCardSource());
            IEnumerator coroutine2 = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c) && c.IsHero), new int?(2), storedResultsAction: storedResultsAction, cardSource: this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            //The Head with the highest HP deals {H} toxic damage to each hero who did not destroy a card this way.
            List<Card> characterCardsWithDestroyed = new List<Card>();
            if (storedResultsAction.Count<DestroyCardAction>() > 0)
            {
                using (List<DestroyCardAction>.Enumerator enumerator = storedResultsAction.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        DestroyCardAction destroy = enumerator.Current;
                        characterCardsWithDestroyed.Add(destroy.CardToDestroy.CharacterCard);
                    }
                }
            }
            coroutine = base.DealDamage(null, (Card c) => !characterCardsWithDestroyed.Contains(c) && c.IsHeroCharacterCard, base.H, DamageType.Toxic, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "the head with the highest HP")));
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