using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MoonShardkeyCardController : OriphelShardkeyCardController
    {
        public MoonShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Shardkey transformation trigger
            base.AddTriggers();

            //"At the end of the villain turn, the villain target with the highest HP deals the hero target with the highest HP 2 energy damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, HighestDamagesHighestResponse, TriggerType.DealDamage);
        }

        private IEnumerator HighestDamagesHighestResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = DealDamageToHighestHP(null, 1, (Card c) => IsHeroTarget(c), (c) => 2, DamageType.Energy, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => IsVillainTarget(c), "The villain target with the highest HP")));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}