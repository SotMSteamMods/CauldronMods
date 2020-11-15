using System;
using System.Collections;
using System.Collections.Generic;

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

        public static string Identifier = "Desolation";

        public DesolationCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Destroy self at end of env. turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse),
                TriggerType.DestroySelf, null, false);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator choiceRoutine = base.DoActionToEachTurnTakerInTurnOrder(
                ttc => ttc.IsHero && !ttc.IsIncapacitatedOrOutOfGame,
                ChoiceResponse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(choiceRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(choiceRoutine);
            }
        }

        private IEnumerator ChoiceResponse(TurnTakerController ttc)
        {
            List<YesNoCardDecision> yesNoResults = new List<YesNoCardDecision>();
            IEnumerator yesNoRoutine = base.GameController.MakeYesNoCardDecision(ttc.ToHero(),
                SelectionType.DiscardCard, base.Card, storedResults: yesNoResults, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(yesNoRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(yesNoRoutine);
            }

            if (DidPlayerAnswerYes(yesNoResults))
            {
                // Discard all cards in hand except 1
                while (ttc.ToHero().NumberOfCardsInHand > 1)
                {
                    IEnumerator discardCard 
                        = this.GameController.SelectAndDiscardCard(ttc.ToHero(), responsibleTurnTaker: ttc.TurnTaker);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(discardCard);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(discardCard);
                    }
                }
            }
            else
            {
                // Opted not to discard, deal damage to hero

                int damageToDeal = Game.H + 1;

                IEnumerator dealDamageRoutine 
                    = this.DealDamage(this.Card, ttc.CharacterCard, damageToDeal, DamageType.Psychic);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageRoutine);
                }
            }
        }
    }
}