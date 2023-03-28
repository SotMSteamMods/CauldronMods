using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class TheresAlwaysABardCardController : DungeonsOfTerrorUtilityCardController
    {
        public TheresAlwaysABardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "active hero"), storedResults, isPutIntoPlay, decisionSources);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        public override void AddTriggers()
        {
            //At the start of their turn, they may use a power, play a card, or destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => GetCardThisCardIsNextTo() != null && tt == GetCardThisCardIsNextTo().Owner, StartOfTurnResponse, new TriggerType[]
            {
                TriggerType.UsePower,
                TriggerType.PlayCard,
                TriggerType.DestroySelf
            });

            //Damage dealt to the hero next to this card is irreducible and increased by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => GetCardThisCardIsNextTo() != null && dd.Target == GetCardThisCardIsNextTo(), 1);
            AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => GetCardThisCardIsNextTo() != null && dd.Target == GetCardThisCardIsNextTo());
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            HeroTurnTakerController httc = FindHeroTurnTakerController(GetCardThisCardIsNextTo().Owner.ToHero());

            //op1: use a power
            var response1 = GameController.SelectAndUsePower(httc, optional: false, cardSource: GetCardSource());
            var op1 = new Function(httc, "Use a power", SelectionType.UsePower, () => response1);

            //op2: play a card
            var response2 = GameController.SelectAndPlayCardFromHand(httc, false, cardSource: GetCardSource());
            var op2 = new Function(httc, "Play a card", SelectionType.PlayCard, () => response2);

            //op3: destroy this card
            var response3 = DestroyThisCardResponse(pca);
            var op3 = new Function(httc, "Destroy this card", SelectionType.DestroyCard, () => response3);

            //Execute
            var options = new Function[] { op1, op2, op3 };
            var selectFunctionDecision = new SelectFunctionDecision(GameController, httc, options, false, cardSource: GetCardSource());
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }
    }
}
