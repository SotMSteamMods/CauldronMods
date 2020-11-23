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

        public static readonly string Identifier = "SniperRifle";

        private const int CardsToDiscard = 2;
        private const int TargetAmount = 1;
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

            int discardNumeral = base.GetPowerNumeral(0, CardsToDiscard);
            IEnumerator discardCardsRoutine = this.GameController.SelectAndDiscardCards(DecisionMaker,
                discardNumeral, false,
                null,
                discardCardActions, false, null, null, null,
                cardCriteria, SelectionType.DiscardCard, this.TurnTaker,
                cardSource: GetCardSource()
            );

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardsRoutine);
            }

            if (DidDiscardCards(discardCardActions, discardNumeral))
            {
                // Discard requirement fulfilled, choose non character card to destroy
                IEnumerator destroyCardRoutine = this.GameController.SelectAndDestroyCard(DecisionMaker,
                        new LinqCardCriteria(card => !card.IsCharacter), false,
                        cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCardRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCardRoutine);
                }
            }
            yield break;
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
            int targetNumeral = GetPowerNumeral(0, TargetAmount);
            int damageNumeral = GetPowerNumeral(1, DamageAmount);

            IEnumerator dealDamageRoutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(base.GameController, selectedCard), damageNumeral,
                DamageType.Projectile, targetNumeral, false, 0,
                additionalCriteria: c => c.IsTarget && c.IsInPlayAndHasGameText,
                cardSource: GetCardSource());

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