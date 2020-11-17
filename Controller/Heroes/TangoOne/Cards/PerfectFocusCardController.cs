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
                NumberOfUses = 1,
                CardDestroyedExpiryCriteria =
                {
                    Card = base.Card
                }
            });

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
            }


            if (!base.HeroTurnTakerController.HasCardsInHand)
            {
                IEnumerator sendMessage = base.GameController.SendMessageAction("No cards in hand to play, skipping", 
                    Priority.High, base.GetCardSource(), null, true);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(sendMessage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(sendMessage);
                }

                yield break;
            }

            // Ask if player wants to play a card
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
            IEnumerator decidePlayCard = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.PlayCard, this.Card, null, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(decidePlayCard);
            }
            else
            {
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