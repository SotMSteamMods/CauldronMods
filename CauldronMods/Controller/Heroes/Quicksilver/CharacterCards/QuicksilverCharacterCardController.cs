using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class QuicksilverCharacterCardController : HeroCharacterCardController
    {
        public QuicksilverCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now.
                        IEnumerator coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController, false, true, false, null, null, null, false, true);
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
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing", true, false, null, null, false), false, null, null, base.GetCardSource(null));
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
                        //Until the start of your next turn increase melee damage dealt by hero targets by 1.
                        IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                        increaseDamageStatusEffect.SourceCriteria.IsHero = new bool?(true);
                        //increaseDamageStatusEffect.DamageTypeCriteria.invertTypes = true;
                        increaseDamageStatusEffect.DamageTypeCriteria.AddType(DamageType.Melee);
                        increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                        IEnumerator coroutine3 = base.AddStatusEffect(increaseDamageStatusEffect, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard a card.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, 1, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If you do, draw 2 cards
            int drawNumeral = GetPowerNumeral(0, 2);
            if (storedResults.Count == 1)
            {
                coroutine = base.DrawCards(base.HeroTurnTakerController, drawNumeral);
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