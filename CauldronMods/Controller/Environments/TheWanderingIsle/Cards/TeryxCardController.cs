using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class TeryxCardController : TheWanderingIsleCardController
    {
        public TeryxCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //This card is indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);

            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //If this card reaches 0HP, the heroes lose.
            base.AddTrigger<DealDamageAction>(dda => dda.Target == this.Card && this.Card.HitPoints <= 0, this.CheckIfGameOverResponse, TriggerType.GameOver, TriggerTiming.After);

            //At the end of the environment turn, the villain target with the highest HP deals Teryx { H + 2} energy damage
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealVillainDamageResponse, TriggerType.DealDamage);
            //Whenever a hero target would deal damage to Teryx, Teryx Instead regains that much HP.
            base.AddPreventDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsHeroTarget && dda.Target == base.Card, (DealDamageAction dda) => base.GameController.GainHP(base.Card, dda.Amount, cardSource: dda.DamageSource.GetCardSource()), new TriggerType[]
            {
                TriggerType.GainHP
            }, true);
        }

        private IEnumerator DealVillainDamageResponse(PhaseChangeAction pca)
        {
            //Find the villain target with the highest HP
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => IsVillainTarget(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Any())
            {
                //that target deals Teryx H + 2 energy damage
                Card highestVillain = storedResults.First();
                coroutine = base.DealDamage(highestVillain, base.Card, base.H + 2, DamageType.Energy, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        private IEnumerator CheckIfGameOverResponse(DealDamageAction dda)
        {
            //the heroes lose
            IEnumerator coroutine = base.GameController.GameOver(EndingResult.EnvironmentDefeat, "Teryx has been destroyed. The island is sinking...",
                actionSource: dda,
                cardSource: GetCardSource());
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

        //This card is indestructible
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }
    }
}
