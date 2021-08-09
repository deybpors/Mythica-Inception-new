using Items_and_Barter_System.Scripts;
using MyBox;
using UnityEngine;

namespace Quest_System
{
    [System.Serializable]
    public class QuestRewardsType
    {
        public RewardTypes rewardType;
        [ConditionalField(nameof(rewardType), false, RewardTypes.items)]
        public ItemObject rewardItem;
        [ConditionalField(nameof(rewardType), false, RewardTypes.experience)]
        public int experienceReward;
        public Sprite icon;
        public string rewardName;
    }
}