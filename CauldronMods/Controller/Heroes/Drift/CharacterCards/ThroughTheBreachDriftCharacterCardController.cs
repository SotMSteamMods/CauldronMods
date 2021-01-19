using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ThroughTheBreachDriftCharacterCardController : DriftSubCharacterCardController
    {
        public ThroughTheBreachDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int cardNumeral = base.GetPowerNumeral(0, 2);

            //Add the top 2 cards of your deck to your shift track, or discard the card from your current shift track space.
            //Cards added to your shift track are placed face up next to 1 of its 4 spaces. Each space may only have 1 card next to it. They are not considered in play.
            //When you discard a card from the track, you may play it or {Drift} may deal 1 target 3 radiant damage.
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One hero may draw a card now.
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
                        //Two heroes may use a power now. If those powers deal damage, reduce that damage by 1.
                        List<SelectCardDecision> selectedHero = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectHeroCharacterCard(base.HeroTurnTakerController, SelectionType.UsePower, selectedHero, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //First Hero
                        coroutine = base.SelectHeroToUsePowerAndModifyIfDealsDamage(base.HeroTurnTakerController, (Func<DealDamageAction, bool> c) => base.AddReduceDamageTrigger((Card card) => true, 1), -1, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt == selectedHero.FirstOrDefault().SelectedCard.Owner));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //Second Hero
                        coroutine = base.SelectHeroToUsePowerAndModifyIfDealsDamage(base.HeroTurnTakerController, (Func<DealDamageAction, bool> c) => base.AddReduceDamageTrigger((Card card) => true, 1), -1, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != selectedHero.FirstOrDefault().SelectedCard.Owner));
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
                        //Move 1 environment card from play to the of its deck.
                        List<SelectCardDecision> cardDecision = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ReturnToDeck, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndHasGameText, "environment"), cardDecision, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        Card selectedCard = cardDecision.FirstOrDefault().SelectedCard;
                        coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedCard, selectedCard.NativeDeck, cardSource: base.GetCardSource());
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
    }
}
