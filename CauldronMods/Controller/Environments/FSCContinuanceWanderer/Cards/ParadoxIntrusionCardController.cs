using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class ParadoxIntrusionCardController : CardController
    {

        public ParadoxIntrusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => c.DoKeywordsContain("time vortex"), "time vortex"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => base.H, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            Func<int> X = () => base.FindCardsWhere((Card c) => c.Location == this.TurnTaker.Trash && c.IsEnvironment && c.IsInTrash && c.DoKeywordsContain("time vortex")).Count();
            coroutine = base.DealDamage(base.Card, (Card c) => IsVillainTarget(c), (Card c) => 2, DamageType.Energy, dynamicNumberOfTargets: X);
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