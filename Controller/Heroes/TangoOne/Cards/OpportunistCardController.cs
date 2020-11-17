﻿using System.Collections;
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

        public static string Identifier = "Opportunist";

        private const int CardsToDraw = 2;
        private const int DamageIncrease = 3;

        public OpportunistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Increase the next damage dealt by {TangoOne} by 3
            IEnumerator increaseDamageRoutine = base.AddStatusEffect(new IncreaseDamageStatusEffect(DamageIncrease)
            {
                SourceCriteria =
                {
                    IsSpecificCard = base.Card.Owner.CharacterCard
                },
                NumberOfUses = 1,
                CardDestroyedExpiryCriteria =
                {
                    Card = base.Card
                }
            });


            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to shuffle trash into deck
            IEnumerator decideTrashShuffleRoutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.ShuffleTrashIntoDeck, this.Card, null, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
                yield return base.GameController.StartCoroutine(decideTrashShuffleRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
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