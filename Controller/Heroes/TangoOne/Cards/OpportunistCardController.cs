using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class OpportunistCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Increase the next damage dealt by {TangoOne} by 3.
        // You may shuffle your trash into your deck.
        // Draw 2 cards.
        //==============================================================

        public static readonly string Identifier = "Opportunist";

        private const int CardsToDraw = 2;
        private const int DamageIncrease = 3;

        public OpportunistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Increase the next damage dealt by {TangoOne} by 3
            var effect = new IncreaseDamageStatusEffect(DamageIncrease);
            effect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            effect.CardSource = Card;
            effect.Identifier = IncreaseDamageIdentifier;
            effect.NumberOfUses = 1;

            IEnumerator increaseDamageRoutine = base.AddStatusEffect(effect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
            }


            if (!base.HeroTurnTakerController.HasCardsWhere(card => card.IsInTrash))
            {
                IEnumerator sendMessage = base.GameController.SendMessageAction("No cards in trash to shuffle into deck, skipping",
                    Priority.High, base.GetCardSource(), null, true);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(sendMessage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(sendMessage);
                }
            }
            else
            {
                List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

                // Ask if player wants to shuffle trash into deck
                IEnumerator decideTrashShuffleRoutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                    SelectionType.ShuffleTrashIntoDeck, this.Card, null, storedYesNoResults, null, GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(decideTrashShuffleRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(decideTrashShuffleRoutine);
                }

                if (base.DidPlayerAnswerYes(storedYesNoResults))
                {
                    // Shuffle trash into deck
                    IEnumerator shuffleTrashIntoDeckRoutine
                        = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, false,
                            null, base.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(shuffleTrashIntoDeckRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(shuffleTrashIntoDeckRoutine);
                    }
                }
            }

            // Draw 2 cards
            IEnumerator drawCardsRoutine = base.DrawCards(this.HeroTurnTakerController, CardsToDraw);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardsRoutine);
            }
        }
    }
}