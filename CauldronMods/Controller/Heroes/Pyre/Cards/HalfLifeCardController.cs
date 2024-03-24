using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class HalfLifeCardController : PyreUtilityCardController
    {
        public HalfLifeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"));
        }
        public override IEnumerator Play()
        {
            //"Search your deck for an equipment card and put it into your hand. 
            var equipmentCriteria = new LinqCardCriteria((Card c) => IsEquipment(c), "equipment");
            var storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = SearchForCards(DecisionMaker, true, false, 1, 1, equipmentCriteria, false, true, false, storedResults: storedResults, shuffleAfterwards: false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //{PyreIrradiate} that card until it leaves your hand. 
            if(DidSelectCard(storedResults))
            {
                coroutine = IrradiateCard(GetSelectedCard(storedResults));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Shuffle your deck. ",
            coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"You may play an equipment card.",
            coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, true, cardCriteria: equipmentCriteria, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"{Pyre} deals each non-hero target 0 energy damage."
            coroutine = DealDamage(CharacterCard, (Card c) => !IsHeroTarget(c), 0, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
