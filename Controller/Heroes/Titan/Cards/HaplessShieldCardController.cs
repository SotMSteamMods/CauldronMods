using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class HaplessShieldCardController : CardController
    {
        public HaplessShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlay && !c.IsHero));
            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            //Select a non-hero target. 
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, choices, storedResults, cardSource: base.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResults.Any())
            {
                //...Each other non-hero target deals that target 1 melee damage.
                coroutine = base.DealDamage((Card c) => c != storedResults.FirstOrDefault().SelectedCard && c.IsInPlay && c.IsTarget && !c.IsHero, (Card c) => c == storedResults.FirstOrDefault().SelectedCard, (Card c) => new int?(1), DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}