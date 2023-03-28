using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class DetectiveSedrickCardController : ResponderCardController
    {
        public DetectiveSedrickCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator PerformActionOnDestroy()
        {
            //1 hero target regains 2HP.
            IEnumerable<Card> choices = FindCardsWhere((Card c) => IsHeroTarget(c) && !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndHasGameText, visibleToCard: GetCardSource());
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, DecisionMaker, SelectionType.GainHP, choices, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision decision) => GameController.GainHP(decision.SelectedCard, 2, cardSource: GetCardSource()));
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

        public override IEnumerator Play()
        {
            //When this card enters play, it deals 1 target 3 projectile damage.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 3, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
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
