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
            SpecialStringMaker.ShowCardThisCardIsNextTo(card);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Place this card next to a target
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText, "targets"), storedResults, isPutIntoPlay, decisionSources);
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
            base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && this.IsUndead(dd.DamageSource.Card) && base.IsThisCardNextToCard(dd.Target));

            //if the card this is next to leaves, have this card fall off
            Card cardThisCardIsNextTo = base.GetCardThisCardIsNextTo(true);
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false, cardThisCardIsNextTo != null && !IsHeroCharacterCard(cardThisCardIsNextTo));
        }
    }
}
