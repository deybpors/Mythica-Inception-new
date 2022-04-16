using Items_and_Barter_System.Scripts;
using MyBox;
using UnityEngine;

namespace Quest_System
{
    [System.Serializable]
    public class QuestRewardsType
    {
        public RewardTypesEnum typeEnumOfReward;
        [ConditionalField(nameof(typeEnumOfReward), false, RewardTypesEnum.Items)]
        public ItemObject rewardItem;
    }
}