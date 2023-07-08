using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class FutureTitanCharacterCardController : TitanBaseCharacterCardController
    {
        public FutureTitanCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
                        //Two targets regain 1 HP each.
                        coroutine = base.GameController.SelectAndGainHP(this.DecisionMaker, 1, false, (Card c) => c.IsInPlay && c.IsTarget, 2, new int?(2), cardSource: base.GetCardSource(null));
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
                        //Destroy an ongoing card.
                        coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c)), false, cardSource: base.GetCardSource());
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
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = null;
            if (FindCard("Titanform").IsInPlayAndHasGameText)
            {
                //If Titanform is in play, 1 target deals {Titan} 1 melee damage. 
                int targetNumeral = base.GetPowerNumeral(0, 1);
                int damageNumeral = base.GetPowerNumeral(1, 1);
                List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
                var targets = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget);
                //Select a target. 
                coroutine = base.GameController.SelectCardsAndStoreResults(base.HeroTurnTakerController,
                                                                        SelectionType.CardToDealDamage,
                                                                        (Card c) => c.IsInPlayAndHasGameText && c.IsTarget,
                                                                        targetNumeral,
                                                                        storedResults,
                                                                        false,
                                                                        targetNumeral,
                                                                        cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(DidSelectCards(storedResults))
                { 
                    //That target (those targets) deals Titan one melee damage
                    coroutine = DealDamage((Card c) => storedResults.FirstOrDefault().SelectCardDecisions.Any((SelectCardDecision scd) => scd.SelectedCard == c),
                                          (Card c) => c == base.CharacterCard,
                                          (Card c) => damageNumeral,
                                          DamageType.Melee);
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
            else
            {
                //Otherwise, play the top card of your deck.
                coroutine = base.GameController.PlayTopCard(base.HeroTurnTakerController, base.TurnTakerController, cardSource: base.GetCardSource());
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