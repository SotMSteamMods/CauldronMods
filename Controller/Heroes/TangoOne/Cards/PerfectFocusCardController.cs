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

        public static readonly string Identifier = "PerfectFocus";

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
            YesNoDecision yesNo = new YesNoDecision(base.GameController, base.HeroTurnTakerController,
                SelectionType.PlayCard, false, cardSource: GetCardSource());

            IEnumerator decidePlayCard = base.GameController.MakeDecisionAction(yesNo, true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(decidePlayCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(decidePlayCard);
            }

            if (yesNo.Answer == null || !yesNo.Answer.Value)
            {
                yield break;
            }



            // Play a card
            IEnumerator playCardRoutine = base.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true);

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