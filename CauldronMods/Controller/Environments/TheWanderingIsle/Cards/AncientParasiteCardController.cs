using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class AncientParasiteCardController : TheWanderingIsleCardController
    {
        public AncientParasiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever this card is dealt damage by a hero target, move it next to that target.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && IsHeroTarget(dd.DamageSource.Card) && !IsThisCardNextToCard(dd.DamageSource) && dd.DidDealDamage, this.NextToResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After);
            //If the card this card is next to leaves play, have this card fall off in the play area
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, true);
            //At the start of the environment turn, if this card is next to a target, it deals that target {H} toxic damage and moves back to the environment play area. Otherwise it deals Teryx {H + 2} toxic damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            Card nextTo = base.GetCardThisCardIsNextTo(true);
            if (nextTo != null)
            {
                //if this card is next to a target, it deals that target {H} toxic damage 
                IEnumerator damageHero = base.DealDamage(base.Card, nextTo, base.H, DamageType.Toxic, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(damageHero);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(damageHero);
                }

                //and moves back to the environment play area.
                IEnumerator moveCard = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.FindEnvironment().TurnTaker.PlayArea,
                    playCardIfMovingToPlayArea: false,
                    actionSource: pca,
                    cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(moveCard);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(moveCard);
                }
            }
            else
            {
                //Otherwise it deals Teryx {H + 2} toxic damage.
                Card teryx = base.FindTeryx();
                if (teryx != null)
                {
                    IEnumerator damageTeryx = base.DealDamage(base.Card, teryx, base.H + 2, DamageType.Toxic, cardSource: base.GetCardSource());
                    if (this.UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(damageTeryx);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(damageTeryx);
                    }
                }

            }
            yield break;
        }

        private IEnumerator NextToResponse(DealDamageAction dd)
        {
            //move it next to that target that dealt damage
            Card card = dd.DamageSource.Card;
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, card.NextToLocation,
                playCardIfMovingToPlayArea: false,
                    actionSource: dd,
                    cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
