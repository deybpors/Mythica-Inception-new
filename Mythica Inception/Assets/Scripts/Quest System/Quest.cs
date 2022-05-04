using System;
using System.Collections.Generic;
using _Core.Others;
using Assets.Scripts.Dialogue_System;
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

        [Space] [Tooltip("Conversation to proceed if ever the current quest succeeds")]
        public Conversation successConversation;

        [Tooltip("Conversation to proceed if ever the current quest fails")]
        public Conversation failConversation;
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
        public Character questGiver;
        public int currentAmount;
        public bool completed;
        public DateTime dateAccepted;

        public PlayerAcceptedQuest(Quest newQuest, Character questGiver, DateTime dateAccepted)
        {
            quest = newQuest;
            this.questGiver = questGiver;
            currentAmount = 0;
            this.dateAccepted = dateAccepted;
            completed = false;
        }
    }

    public enum RewardTypesEnum
    {
        Items, Experience,
    }
}
