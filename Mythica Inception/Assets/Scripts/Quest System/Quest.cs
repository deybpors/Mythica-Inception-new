using System.Collections.Generic;
using _Core.Others;
using _Core.Player;
using Items_and_Barter_System.Scripts;
using UnityEngine;

namespace Quest_System
{
    [CreateAssetMenu(menuName = "Quest System/New Quest")]
    public class Quest : ScriptableObjectWithID
    {
        public string title;
        [TextArea(5,10)]
        public string description;
        public List<Reward> rewards;
        public QuestGoal goal;
        [Space]
        [Tooltip("Quests to proceed if ever the current quest succeeds")]
        public Quest[] succeedQuests;

        [Tooltip("Quests to proceed if ever the current quest fails")]
        public Quest[] failedQuests;
    }

    [System.Serializable]
    public class Reward
    {
        public QuestRewardsType rewardsType;
        public int value;
    }

    [System.Serializable]
    public class PlayerAcceptedQuest
    {
        public Quest quest;
        public int currentValue;

        public PlayerAcceptedQuest(Quest newQuest)
        {
            quest = newQuest;
            currentValue = 0;
        }
    }

    public enum RewardTypes
    {
        Items, Experience,
    }
}
