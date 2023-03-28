using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.TangoOne
{
    public class WetWorkCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Shuffle 2 cards from each trash back into their decks.
        // You may deal 1 target 2 melee damage.
        //==============================================================

        public static readonly string Identifier = "WetWork";

        private const int CardsToMoveFromTrash = 2;
        private const int DamageAmount = 2;

        public WetWorkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Shuffle 2 cards from each trash back into their decks
            IEnumerator coroutine = GameController.SelectLocationsAndDoAction(DecisionMaker, SelectionType.ShuffleCardFromTrashIntoDeck, loc => (loc.IsTrash || loc.IsSubTrash) && loc.IsRealTrash && !loc.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && GameController.IsLocationVisibleToSource(loc, GetCardSource()), SelectCardsToShuffle, cardSource: GetCardSource()) ;
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // You may deal 1 target 2 melee damage
            IEnumerator dealDamageRoutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(base.GameController, CharacterCard), DamageAmount,
                DamageType.Melee, 1, false, 0,
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }

        private IEnumerator SelectCardsToShuffle(Location trash)
        {
            TurnTaker turnTaker = trash.OwnerTurnTaker;
            TurnTakerController turnTakerController = FindTurnTakerController(turnTaker);
            HeroTurnTakerController decisionMaker = IsHero(turnTaker) ? turnTakerController.ToHero() : DecisionMaker;

            SelectCardsDecision scsd = new SelectCardsDecision(GameController, decisionMaker, c => c.Location == trash, SelectionType.ShuffleCardFromTrashIntoDeck,
                   numberOfCards: CardsToMoveFromTrash,
                   isOptional: false,
                   requiredDecisions: CardsToMoveFromTrash,
                   cardSource: GetCardSource());

            Location deck = trash.IsSubTrash ? turnTaker.FindSubDeck(trash.Identifier) : turnTaker.Deck;

            IEnumerator coroutine = GameController.SelectCardsAndDoAction(scsd, scd => GameController.MoveCard(decisionMaker, scd.SelectedCard, deck, cardSource: GetCardSource()), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.ShuffleLocation(deck, cardSource: GetCardSource());
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