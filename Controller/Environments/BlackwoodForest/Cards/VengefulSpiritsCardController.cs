using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class VengefulSpiritsCardController : CardController
    {
        //==============================================================
        // At the start of the environment turn, shuffle the villain trash
        // and reveal cards until a target is revealed.
        // Put that target into play, then this card deals that
        // target 2 infernal damage.
        // At the end of the environment turn, each player may
        // discard 2 cards to destroy this card.
        //==============================================================

        public static readonly string Identifier = "VengefulSpirits";

        private const int DamageToDeal = 2;
        private const int NumberOfCardMatches = 1;
        private const int NumberOfCardsToDiscard = 2;


        public VengefulSpiritsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCardsAtLocation(FindLocationsWhere(location => location.IsRealTrash && location.IsVillain && GameController.IsLocationVisibleToSource(location, GetCardSource())).First(), new LinqCardCriteria(c => c.IsTarget, "target", useCardsSuffix: false, singular: "target", plural: "targets"));       
        }

        public override void AddTriggers()
        {
            // At the start of the environment turn, shuffle the villain trash and reveal cards until a target is revealed.
            // Put that target into play, then this card deals that target 2 infernal damage.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnShuffleVillianTrashResponse,
                TriggerType.ShuffleDeck);

            // At the end of the environment turn, each player may discard 2 cards to destroy this card.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnOptionalDestruction,
                TriggerType.DestroyCard);

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnShuffleVillianTrashResponse(PhaseChangeAction pca)
        {
            Location villainTrash = FindLocationsWhere(location => location.IsRealTrash && location.IsVillain && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();

            // Shuffle villain trash
            IEnumerator shuffleTrashRoutine = GameController.ShuffleLocation(villainTrash);

            // Reveal cards until target is revealed (if any)
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine = GameController.RevealCards(TurnTakerController, villainTrash, card => card.IsTarget, NumberOfCardMatches, revealedCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashRoutine);
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashRoutine);
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            Card matchedCard = GetRevealedCards(revealedCards).FirstOrDefault(c => c.IsTarget);
            List<Card> otherCards = GetRevealedCards(revealedCards).Where(c => !c.IsTarget).ToList();
            if (otherCards.Any())
            {
                // Put non matching revealed cards back in the trash
                IEnumerator returnCardsRoutine = GameController.MoveCards(DecisionMaker, otherCards, villainTrash, cardSource: GetCardSource());

                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(returnCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(returnCardsRoutine);
                }
            }

            if (matchedCard != null)
            {
                // Eligible card found, put it into play and deal it 2 infernal damage
                List<bool> wasCardPlayed = new List<bool>();
                IEnumerator coroutine = GameController.PlayCard(TurnTakerController, matchedCard, isPutIntoPlay: true, wasCardPlayed: wasCardPlayed, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (wasCardPlayed.FirstOrDefault() == true)
                {
                    coroutine = DealDamage(Card, matchedCard, DamageToDeal, DamageType.Infernal);
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

        private IEnumerator EndOfTurnOptionalDestruction(PhaseChangeAction pca)
        {
            List<DiscardCardAction> discardedCards = new List<DiscardCardAction>();
            IEnumerator discardCardsRoutine = this.GameController.EachPlayerDiscardsCards(0, NumberOfCardsToDiscard, discardedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardsRoutine);
            }

            if (discardedCards.Count != (this.Game.H * NumberOfCardsToDiscard))
            {
                yield break;
            }

            // Required cards discarded, destroy this card
            IEnumerator destroyRoutine = base.GameController.DestroyCard(HeroTurnTakerController, Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }
        }
    }
}