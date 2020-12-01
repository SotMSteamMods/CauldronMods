using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class OniTitanCharacterCardController : HeroCharacterCardController
    {
        public OniTitanCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
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
                        //One player may use a power now.
                        coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        List<DiscardCardAction> list = new List<DiscardCardAction>();
                        //One hero may discard a card to reduce damage dealt to them by 1 until the start of your turn.
                        coroutine = base.GameController.SelectHeroToDiscardCard(base.HeroTurnTakerController, storedResultsDiscard: list, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (list.Any())
                        {
                            List<Card> cards = list.FirstOrDefault().HeroTurnTakerController.CharacterCards.ToList();
                            ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(1)
                            {
                                TargetCriteria = { IsOneOfTheseCards = cards }
                            };
                            statusEffect.UntilStartOfNextTurn(base.TurnTaker);
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
                        break;
                    }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Titan regains 1HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Search your deck, trash, and hand for Titanform and play it.
            coroutine = base.GameController.PlayCard(base.TurnTakerController, FindCard("Titanform"), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle your deck.
            coroutine = base.ShuffleDeck(base.HeroTurnTakerController, base.TurnTaker.Deck);
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