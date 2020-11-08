using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Necro
{
    public class TalismanCardController : NecroCardController
    {
        public TalismanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Place this card next to a target
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget, "targets"), storedResults, isPutIntoPlay, decisionSources);
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
            //That target is immune to damage from undead targets.
            base.AddImmuneToDamageTrigger((DealDamageAction dd) => this.IsUndead(dd.DamageSource.Card) && base.IsThisCardNextToCard(dd.Target));
            base.AddTrigger<DestroyCardAction>(dca => IsThisCardNextToCard(dca.CardToDestroy.Card), dca => dca.GameController.MoveCard(dca.DecisionMaker, this.Card, this.Card.Owner.PlayArea, actionSource: dca, playCardIfMovingToPlayArea: false, doesNotEnterPlay: true), TriggerType.MoveCard, TriggerTiming.After, ignoreBattleZone: true);
        }
    }
}
