using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class WarpBridgeCardController : NightloreCitadelUtilityCardController
    {
        public WarpBridgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(c => c.IsInPlayAndHasGameText && !c.IsCharacter && c != Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "", useCardsSuffix: false, singular: "non-character card in play", plural: "non-character cards in play"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.ShuffleCardIntoDeck,
                TriggerType.PlayCard
            });

            //Then, if Rogue Constellation is in play, destroy this card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroyCard, additionalCriteria: (PhaseChangeAction pca) => IsRogueConstellationInPlay());
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //select 1 non-character card in play other than this one
            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>() ;
            IEnumerable<Card> cardList = from c in this.FindCardsWhere((Card c) => c != Card && c.IsInPlay && !c.IsCharacter && c.IsRealCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()))
                                         orderby c.Owner.Name
                                         select c;
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ShuffleCardIntoDeck, cardList, storedDecision, false, maintainCardOrder: true, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectCard(storedDecision))
            {
                //shuffle it back into its associated deck.
                Card selectedCard = GetSelectedCard(storedDecision);
                coroutine = GameController.MoveCard(TurnTakerController, selectedCard, selectedCard.NativeDeck, showMessage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }


                //If a card leaves play this way, play the top card of the associated deck. 
                if (!selectedCard.IsInPlayAndHasGameText)
                {
                    coroutine = ShuffleDeck(DecisionMaker, selectedCard.NativeDeck);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, selectedCard.NativeDeck, cardSource: GetCardSource());
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
            yield break;
        }
    }
}
