using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    public abstract class CeladrochPillarCardController : CardController
    {
        private readonly int[] _rewardThresholds;
        private readonly TriggerType _rewardType;

        protected CeladrochPillarCardController(Card card, TurnTakerController turnTakerController, TriggerType rewardType) : base(card, turnTakerController)
        {
            _rewardThresholds = GetRewardThresholds();
            _rewardType = rewardType;

            SpecialStringMaker.ShowSpecialString(RemainingRewardsSpecialString);
            SpecialStringMaker.ShowSpecialString(HpTilNextRewardSpecialString).Condition = () => HpTilNextReward() > 0;
        }

        // This card may not regain HP.
        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            if (gameAction is GainHPAction hpAction && hpAction.HpGainer == Card)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }

        public override void AddTriggers()
        {
            //Reduce damge dealt to Celadroch by 1.
            AddReduceDamageTrigger(c => c.IsTarget && c.Identifier == "Celadroch", 1);

            //reward trigger
            AddTrigger<DealDamageAction>(dda => dda.Target == Card && dda.DidDealDamage, PillarCardRewardResponse, _rewardType, TriggerTiming.After);

            //when this card would leave play, remove it from the game.
            AddTrigger<MoveCardAction>(mca => mca.CardToMove == Card && mca.Origin.IsPlayArea && !mca.Destination.HighestRecursiveLocation.IsInPlay, MoveOutOfPlayResponse, TriggerType.CancelAction, TriggerTiming.Before);
        }

        private IEnumerator PillarCardRewardResponse(DealDamageAction action)
        {
            return null;
        }

        //When this card would leave play, remove it from the game
        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(TurnTaker.OutOfGame, false, true, true);
        }

        protected abstract IEnumerator SelectHeroAndGrantReward();

        private string RemainingRewardsSpecialString()
        {
            int num = NumberOfRemainingRewards();
            switch (num)
            {
                case 0: return $"{Card.Title} has used all it's rewards.";
                case 1: return $"{Card.Title} has 1 reward remaining.";
                default:
                    return $"{Card.Title} has {num} rewards remaining.";
            }
        }

        private string HpTilNextRewardSpecialString()
        {
            int num = HpTilNextReward();
            switch (num)
            {
                case 0: return "";
                case 1: return $"{Card.Title} will give a reward after losing 1 more HP.";
                default:
                    return $"{Card.Title} will give a reward after losing {num} more HPs.";
            }
        }


        private int NumberOfRemainingRewards()
        {
            return 0;
        }

        private int HpTilNextReward()
        {
            return 0;
        }

        private int NumberOfRewardTriggers(int beforeHp, int afterHp)
        {
            return 0;
        }

        private int[] GetRewardThresholds()
        {
            return new int[0];
        }


        private IEnumerator MoveOutOfPlayResponse(MoveCardAction action)
        {
            return null;
        }






    }
}