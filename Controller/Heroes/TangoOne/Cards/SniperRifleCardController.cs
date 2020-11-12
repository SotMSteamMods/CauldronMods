using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class SniperRifleCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Power 1: Discard 2 Critical cards. If you do, destroy a non-character card in play.
        // Power 2: Deal 1 target 2 projectile damage.
        //==============================================================

        public static string Identifier = "SniperRifle";

        private const int CardsToDiscard = 2;
        private const int DamageAmount = 2;

        public SniperRifleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator powerRoutine = null;
            switch (index)
            {
                case 0:
                    powerRoutine = GetPower1();
                    break;

                case 1:
                    powerRoutine = GetPower2();
                    break;
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(powerRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(powerRoutine);
            }
        }

        private IEnumerator GetPower1()
        {
            // Discard 2 Critical cards. If you do, destroy a non-character card in play
            List<DiscardCardAction> discardCardActions = new List<DiscardCardAction>();
            LinqCardCriteria cardCriteria = new LinqCardCriteria(IsCritical, "critical cards", false);

            IEnumerator discardCardsRoutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 
                CardsToDiscard, false,
                null,
                discardCardActions, false, null, null, null, 
                cardCriteria, SelectionType.DiscardCard, this.TurnTaker, null
            );

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardsRoutine);
            }

            if (discardCardActions.Count() != CardsToDiscard)
            {
                yield break;
            }

            // Discard requirement fulfilled, choose non character card to destroy
            IEnumerator destroyCardRoutine 
                = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, 
                    new LinqCardCriteria(card => !card.IsCharacter), false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }
        }

        private IEnumerator GetPower2()
        {
            // Deal 1 target 2 projectile damage.

            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();
            IEnumerator selectOwnCharacterRoutine = base.SelectOwnCharacterCard(selectCardResults, SelectionType.HeroToDealDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectOwnCharacterRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectOwnCharacterRoutine);
            }

            Card selectedCard = GetSelectedCard(selectCardResults);
            int powerNumeral = GetPowerNumeral(0, DamageAmount);

            IEnumerator dealDamageRoutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(base.GameController, selectedCard), powerNumeral,
                DamageType.Projectile, 1, false, 0,
                additionalCriteria: ((Func<Card, bool>)(c => c.IsTarget && c.IsInPlay)),
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
    }
}