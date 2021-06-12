using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class CorruptionCardController : TheStrangerBaseCardController
    {
        #region Constructors

        public CorruptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //Draw up to 4 cards.
            List<DrawCardAction> storedResultsDraw = new List<DrawCardAction>();
            int i;
            for (i = 0; i < 4; i++)
            {
                YesNoAmountDecision yesNo = new YesNoAmountDecision(GameController, DecisionMaker, SelectionType.DrawCard, 1, cardSource: GetCardSource())
                {
                    ExtraInfo = () => $"Cards drawn so far: {i}"
                };

                IEnumerator coroutine = GameController.MakeDecisionAction(yesNo);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (GameController.DidAnswerYes(yesNo))
                {
                    IEnumerator drawCard = base.DrawCard(this.HeroTurnTaker, cardsDrawn: storedResultsDraw);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(drawCard);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(drawCard);
                    }
                }
                else
                {
                    break;
                }
            }

            //For each card drawn this way, {TheStranger} deals himself 1 toxic damage.
            int numberOfCardsDrawn = base.GetNumberOfCardsDrawn(storedResultsDraw);
            for (i = 0; i < numberOfCardsDrawn; i++)
            {
                IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Toxic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

        }
        #endregion Methods
    }
}