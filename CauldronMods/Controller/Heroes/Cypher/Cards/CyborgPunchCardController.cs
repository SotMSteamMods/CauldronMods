using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CyborgPunchCardController : CypherBaseCardController
    {
        //==============================================================
        // You may move 1 Augment in play next to a new hero.
        // One augmented hero deals 1 target 1 melee damage and draws a card now.
        //==============================================================

        public static string Identifier = "CyborgPunch";

        private const int DamageToDeal = 1;

        public CyborgPunchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringAugmentsInPlay();
        }

        public override IEnumerator Play()
        {
            // You may move 1 Augment in play next to a new hero.
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker,
                SelectionType.MoveCardNextToCard, GetAugmentsInPlay(), true, cardSource: GetCardSource());

            IEnumerator routine = base.GameController.SelectCardAndDoAction(scd, MoveInPlayAugment);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // One augmented hero deals 1 target 1 melee damage and draws a card now.
            List<SelectCardDecision> storedHeroCard = new List<SelectCardDecision> { };
            routine = this.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroToDealDamage, base.AugmentedHeroCharacterCardCriteria(), storedHeroCard, false, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!DidSelectCard(storedHeroCard))
            {
                yield break;
            }

            var selectedHero = storedHeroCard.FirstOrDefault().SelectedCard;
            var heroController = FindHeroTurnTakerController(selectedHero.Owner.ToHero());

            routine = base.GameController.SelectTargetsAndDealDamage(heroController,
                new DamageSource(base.GameController, selectedHero), DamageToDeal, DamageType.Melee, 1,
                false, 1, cardSource: GetCardSource());

            IEnumerator routine2 = base.GameController.DrawCard(heroController.HeroTurnTaker, false,
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
                yield return base.GameController.StartCoroutine(routine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
                base.GameController.ExhaustCoroutine(routine2);
            }
        }
    }
}