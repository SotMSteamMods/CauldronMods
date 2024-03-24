using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class WorldShardkeyCardController : OriphelShardkeyCardController
    {
        public WorldShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Shardkey Transformation trigger
            base.AddTriggers();

            //"At the end of the villain turn, the villain target with the highest HP deals the hero target with the lowest HP 2 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, HighestDamagesLowestResponse, TriggerType.DealDamage);
        }

        private IEnumerator HighestDamagesLowestResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = DealDamageToLowestHP(null, 1, (Card c) => IsHeroTarget(c), (c) => 2, DamageType.Melee, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => IsVillainTarget(c), "The villain target with the highest HP")));
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