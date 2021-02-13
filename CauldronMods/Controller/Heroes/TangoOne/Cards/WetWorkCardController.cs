using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

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
            IEnumerator shuffleRoutine = base.DoActionToEachTurnTakerInTurnOrder(ttc => !ttc.IsIncapacitatedOrOutOfGame && ttc.IsLocationVisible(ttc.TurnTaker.Trash, GetCardSource()), MoveCardToDeckResponse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleRoutine);
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

        private IEnumerator MoveCardToDeckResponse(TurnTakerController turnTakerController)
        {
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            //allow each hero to choose for themselves, TangoOne decides for the villain and environment deck.
            HeroTurnTakerController decisionMaker = turnTaker.IsHero ? turnTakerController.ToHero() : this.DecisionMaker;

            var scsd = new SelectCardsDecision(GameController, decisionMaker, c => c.Location == turnTaker.Trash, SelectionType.ShuffleCardFromTrashIntoDeck,
                numberOfCards: CardsToMoveFromTrash,
                isOptional: false,
                requiredDecisions: CardsToMoveFromTrash,
                cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(scsd, scd => GameController.MoveCard(decisionMaker, scd.SelectedCard, turnTaker.Deck, cardSource: GetCardSource()), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.ShuffleLocation(turnTaker.Deck, cardSource: GetCardSource());
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