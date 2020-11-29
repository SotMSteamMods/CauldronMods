using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class RenegadeQuicksilverCharacterCardController : QuicksilverSubCharacterCardController
    {
        public RenegadeQuicksilverCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SetCardProperty(base.GeneratePerTargetKey("Incap2NextTimeHeroIsDealtDamage", base.CharacterCard), false);
        }

        private const string Incap2NextTimeHeroIsDealtDamage = "Incap2NextTimeHeroIsDealtDamage";
        private int Incap2Count = 0;

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        //The next time a hero is dealt damage, they may play a card.
                        this.Incap2Count++;
                        break;
                    }
                case 2:
                    {
                        //Increase all damage dealt by 1 until the start of your next turn.
                        IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
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
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Search your deck for Iron Retort and put it into your hand. 
            coroutine = base.SearchForCards(base.HeroTurnTakerController, true, false, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "IronRetort"), false, true, false);
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

        public override void AddTriggers()
        {
            //The next time a hero is dealt damage, they may play a card.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => base.CharacterCard.IsIncapacitated && this.Incap2Count > 0 && action.Target.IsHero, this.PlayCardResponse, TriggerType.PlayCard, TriggerTiming.After);
        }

        private IEnumerator PlayCardResponse(DealDamageAction action)
        {
            //...they may play a card
            IEnumerator coroutine = base.GameController.SelectAndPlayCardsFromHand(base.GameController.FindHeroTurnTakerController(action.Target.Owner.ToHero()), this.Incap2Count, true, cardSource: base.GetCardSource());
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