using Items_and_Barter_System.Scripts;
using MyBox;
using UnityEngine;

namespace Quest_System
{
    [System.Serializable]
    public class QuestRewardsType
    {
        public RewardTypes typeOfReward;
        [ConditionalField(nameof(typeOfReward), false, RewardTypes.Items)]
        public ItemObject rewardItem;
        [ConditionalField(nameof(typeOfReward), false, RewardTypes.Experience)]
        public int experienceReward;
        public Sprite icon;
        public string rewardName;
    }
}