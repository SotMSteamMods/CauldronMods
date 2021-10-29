using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class DesolationCardController : CardController
    {
        //==============================================================
        // When this card enters play, each hero must discard all but 1 card,
        // or this card deals that hero {H + 1} psychic damage.
        // At the end of the environment turn, destroy this card.
        //==============================================================

        public static readonly string Identifier = "Desolation";

        public DesolationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Destroy self at end of env. turn
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);

            base.AddTriggers();
        }

        private IEnumerator DiscardAllBut1CardResponse(HeroTurnTakerController httc)
        {
            List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(httc, SelectionType.Custom, new LinqCardCriteria((Card c) => c.Location.IsHand && c.Location.OwnerTurnTaker == httc.TurnTaker), cardDecisions, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(cardDecisions))
            {
                yield break;
            }

            Card selectedCard = GetSelectedCard(cardDecisions);

            coroutine = GameController.SendMessageAction($"{Card.Title} discards all cards in {httc.HeroTurnTaker.Hand.GetFriendlyName()} except {selectedCard.Title}.", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            

            coroutine = GameController.DiscardCards(httc, httc.HeroTurnTaker.Hand.Cards.Where((Card c) => c != selectedCard), cardSource: GetCardSource());
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

        private IEnumerator DealPsychicDamageResponse(HeroTurnTakerController httc)
        {
            List<Card> result = new List<Card>();
            var coroutine = FindCharacterCardToTakeDamage(httc.TurnTaker, result, Card, H + 1, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamage(Card, result.First(), H + 1, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator Play()
        {
            IEnumerable<Function> Functions(HeroTurnTakerController httc) => new Function[2]
            {
                new Function(httc, "Discard all but 1 card", SelectionType.DiscardCard,
                    () => DiscardAllBut1CardResponse(httc),
                    httc.HeroTurnTaker.Hand.HasCards),

                new Function(httc, $"Take {H + 1} psychic damage", SelectionType.DealDamage,
                    () => DealPsychicDamageResponse(httc))
            };

            IEnumerator coroutine = EachPlayerSelectsFunction(h => !h.IsIncapacitatedOrOutOfGame, Functions, Game.H, null, h => h.Name + " has no cards in their hand.");
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select a card to keep in your hand", "They are selecting a card to keep in their hand", "Vote for a card they should keep in their hand", "keep a card in hand");

        }
    }
}
