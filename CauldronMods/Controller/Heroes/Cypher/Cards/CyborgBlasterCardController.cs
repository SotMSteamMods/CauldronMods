using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CyborgBlasterCardController : CypherBaseCardController
    {
        //==============================================================
        // You may move 1 Augment in play next to a new hero.
        // One augmented hero deals 1 target 2 lightning damage.
        //==============================================================

        public static string Identifier = "CyborgBlaster";

        private const int DamageToDeal = 2;

        public CyborgBlasterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringAugmentsInPlay();
        }

        public override IEnumerator Play()
        {
            // You may move 1 Augment in play next to a new hero.
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker,
                SelectionType.Custom, GetAugmentsInPlay().Where(c => GameController.IsCardVisibleToCardSource(c, GetCardSource())), true, cardSource: GetCardSource());

            IEnumerator routine = base.GameController.SelectCardAndDoAction(scd, MoveInPlayAugment);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // One augmented hero deals 1 target 2 lightning damage.
            //List<SelectTurnTakerDecision> sttd = new List<SelectTurnTakerDecision>();
            //routine = this.GameController.SelectHeroTurnTaker(this.HeroTurnTakerController, SelectionType.CardToDealDamage, false, false, sttd,
            //    new LinqTurnTakerCriteria(tt => GetAugmentedHeroTurnTakers().Contains(tt)), cardSource: GetCardSource());

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

            var selectedHero = GetSelectedCard(storedHeroCard);
            var heroController = FindHeroTurnTakerController(selectedHero.Owner.ToHero());

            routine = base.GameController.SelectTargetsAndDealDamage(heroController,
                new DamageSource(base.GameController, selectedHero), DamageToDeal, DamageType.Lightning, 1, 
                false, 1, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select an Augment in play to move next to a new hero.", "Selecting an Augment in play to move next to a new hero.", "Vote for an Augment in play to move next to a new hero.", "move an Augment in play next to a new hero");

        }
    }
}