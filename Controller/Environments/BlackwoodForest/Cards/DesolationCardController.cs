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

        public static readonly string Identifier = "Desolation";

        public DesolationCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Destroy self at end of env. turn
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker,
                base.DestroyThisCardResponse,
                TriggerType.DestroySelf);

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
            int cardsToDiscard = ttc.ToHero().NumberOfCardsInHand - 1;

            SelectCardsDecision selectCardsDecision = new SelectCardsDecision(base.GameController, ttc.ToHero(), 
                c => c.Location == ttc.ToHero().HeroTurnTaker.Hand, 
                SelectionType.DiscardCard, cardsToDiscard, true,
                cardsToDiscard, true, cardSource: base.GetCardSource());

            IEnumerator scada = base.GameController.SelectCardsAndDoAction(selectCardsDecision, 
                d => base.GameController.MoveCard(ttc, d.SelectedCard, ttc.ToHero().HeroTurnTaker.Trash, false, false, 
                    true, null, true, new List<IDecision> { d }, isDiscard: true, cardSource: base.GetCardSource()),
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(scada);
            }
            else
            {
                base.GameController.ExhaustCoroutine(scada);
            }

            if (selectCardsDecision.Completed)
            {
                yield break;
            }

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