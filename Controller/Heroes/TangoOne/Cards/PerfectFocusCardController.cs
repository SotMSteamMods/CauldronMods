using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PerfectFocusCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Increase the next damage dealt by {TangoOne} by 3.
        // You may play a card.
        //==============================================================

        public static string Identifier = "PerfectFocus";

        private const int DamageIncrease = 3;

        public PerfectFocusCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
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
                NumberOfUses = new int?(1),
                CardDestroyedExpiryCriteria =
                {
                    Card = base.Card
                }
            }, true);

            // Ask if player wants to play a card
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
            IEnumerator decidePlayCard = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.PlayCard, this.Card, null, storedYesNoResults, null, GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
                yield return base.GameController.StartCoroutine(decidePlayCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
                base.GameController.ExhaustCoroutine(decidePlayCard);
            }


            if (!base.DidPlayerAnswerYes(storedYesNoResults))
            {
                yield break;
            }

            // Play a card
            IEnumerator playCardRoutine = base.SelectAndPlayCardFromHand(this.HeroTurnTakerController);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }
    }
}