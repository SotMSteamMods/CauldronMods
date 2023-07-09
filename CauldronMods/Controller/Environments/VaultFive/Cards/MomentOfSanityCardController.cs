using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class MomentOfSanityCardController : VaultFiveUtilityCardController
    {
        public MomentOfSanityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddAsPowerContributor();
        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy {H - 2} ongoing cards.
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "ongoing"), Game.H - 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            //Heroes gain the following power:
            //Power: Discard 1 Artifact card. If you do, play a card or destroy 1 environment card. Then destroy this card.
            if (cardController.Card != null && IsHeroCharacterCard(cardController.Card) && cardController.Card.IsInPlayAndHasGameText && cardController.Card.IsRealCard && !cardController.Card.IsIncapacitatedOrOutOfGame)
            {
                return new Power[1]
                {
                    new Power(cardController.HeroTurnTakerController, cardController, "Discard 1 Artifact card. If you do, play a card or destroy 1 environment card. Then destroy this card.", DiscardAndDoSomethingResponse(cardController), 0, null, GetCardSource())
                };
            }
            return null;
        }

        private IEnumerator DiscardAndDoSomethingResponse(CardController heroCharacterCardController)
        {
            Card heroCharacterCard = heroCharacterCardController.Card;
            //Discard 1 Artifact card...
            int numArtifact = GetPowerNumeral(0, 1);
            int numEnvironmentCards = GetPowerNumeral(1, 1);

            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            TurnTaker owner = heroCharacterCard.Owner;
            if (owner == null || !IsHero(owner))
            {
                yield break;
            }
            HeroTurnTakerController hero = FindHeroTurnTakerController(owner.ToHero());
            IEnumerator coroutine = base.GameController.SelectAndDiscardCards(hero, numArtifact, false, numArtifact, cardCriteria: new LinqCardCriteria((Card c) => IsArtifact(c), "artifact"), storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...If you do...
            if (DidDiscardCards(storedResults))
            {
                //...play a card...
                var response1 = SelectAndPlayCardFromHand(hero);
                var op1 = new Function(this.DecisionMaker, $"Play a card.", SelectionType.PlayCard, () => response1);

                //...or destroy 1 environment card...
                var response2 = GameController.SelectAndDestroyCards(hero, new LinqCardCriteria((Card c) => c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, heroCharacterCardController.GetCardSource()), "environment"), numEnvironmentCards, cardSource: GetCardSource());
                var op2 = new Function(this.DecisionMaker, $"Destroy {numEnvironmentCards} environment card.", SelectionType.DestroyCard, () => response2,
                    onlyDisplayIfTrue: base.GameController.GetAllCards().Any(c => c.IsEnvironment && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, heroCharacterCardController.GetCardSource())));

                //Execute
                var options = new Function[] { op1, op2 };
                var selectFunctionDecision = new SelectFunctionDecision(base.GameController, this.DecisionMaker, options, false, cardSource: base.GetCardSource());
                coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...Then destroy this card.
                coroutine = DestroyThisCardResponse(null);
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
