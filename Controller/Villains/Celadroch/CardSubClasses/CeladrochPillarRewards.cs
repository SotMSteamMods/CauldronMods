using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cauldron.Celadroch
{
    public class CeladrochPillarRewards
    {
        private readonly int[] _rewardThresholds;

        public CeladrochPillarRewards(int H)
        {
            if (H <= 3)
                _rewardThresholds = new[] { 21, 17, 13, 9, 5, 1, int.MinValue };
            if (H == 4)
                _rewardThresholds = new[] { 20, 15, 10, 5, 0, int.MinValue };
            if (H >= 5)
                _rewardThresholds = new[] { 19, 13, 7, 1, int.MinValue };
        }

        public int RemainingRewards(int hp)
        {
            int index = 0;
            while (index < _rewardThresholds.Length && !(hp > _rewardThresholds[index]))
            {

                index++;
            }
            return _rewardThresholds.Length - index - 1;
        }

        public int HpTillNextTrigger(int hp)
        {
            int index = 0;
            while (index < _rewardThresholds.Length && !(hp > _rewardThresholds[index]))
            {
                index++;
            }

            if (_rewardThresholds[index] == int.MinValue)
                return 0;

            return hp - _rewardThresholds[index];
        }

        public int NumberOfTriggers(int beforeHp, int afterHp)
        {
            if (afterHp >= beforeHp)
                return 0;

            int beforeIndex = 0;
            while (beforeIndex < _rewardThresholds.Length && !(beforeHp > _rewardThresholds[beforeIndex]))
            {
                beforeIndex++;
            }

            int afterIndex = beforeIndex;
            while (afterIndex < _rewardThresholds.Length && !(afterHp > _rewardThresholds[afterIndex]))
            {
                afterIndex++;
            }

            return afterIndex - beforeIndex;
        }

        public override string ToString()
        {
            return $"{{{string.Join(", ", _rewardThresholds.Select(v => v.ToString()).ToArray())}}}";
        }
    }
}
