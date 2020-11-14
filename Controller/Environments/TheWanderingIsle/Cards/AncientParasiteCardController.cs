using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class AncientParasiteCardController : CardController
    {
        public AncientParasiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever this card is dealt damage by a hero target, move it next to that target.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.Card != null && dd.DamageSource.Card.IsTarget && dd.DamageSource.Card.IsHero, new Func<DealDamageAction, IEnumerator>(this.NextToResponse), new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
            //If the card this card is next to leaves play, have this card fall off in the play area
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, true);
            //At the start of the environment turn, if this card is next to a target, it deals that target {H} toxic damage and moves back to the environment play area. Otherwise it deals Teryx {H + 2} toxic damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {

            Card nextTo = base.GetCardThisCardIsNextTo(true);
            if (nextTo != null)
            {
                //if this card is next to a target, it deals that target {H} toxic damage 
                IEnumerator damageHero = base.DealDamage(base.Card, nextTo, base.H, DamageType.Toxic, false, false, false, null, null, null, false, base.GetCardSource(null));
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(damageHero);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(damageHero);
                }

                //and moves back to the environment play area.
                IEnumerator moveCard = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.FindEnvironment(null).TurnTaker.PlayArea, false, false, false, null, false, null, null, null, false, false, null, false, false, false, false, base.GetCardSource(null));
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
                if (IsTeryxInPlay(base.TurnTakerController))
                {
                    Card teryx = base.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsTeryx(c)).First();
                    IEnumerator damageTeryx = base.DealDamage(base.Card, teryx, base.H + 2, DamageType.Toxic, false, false, false, null, null, null, false, base.GetCardSource(null));
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
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, card.NextToLocation, false, false, true, null, false, null, null, null, false, false, null, false, false, false, false, base.GetCardSource(null));
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

        private bool IsTeryxInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsTeryx(c));
            var numCardsInPlay = cardsInPlay.Count();

            return numCardsInPlay > 0;
        }
        private bool IsTeryx(Card card)
        {
            return card.Identifier == "Teryx";
        }
    }
}
