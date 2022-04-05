using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Monster_System;
using UnityEngine;

namespace Quest_System
{
    public class QuestManager : MonoBehaviour
    {
        public Quest questSelected;
        //public void UpdateKillQuest()
        //{
        //    var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
        //    foreach (var playerAcceptedQuest in playerQuests.Where(playerAcceptedQuest => playerAcceptedQuest.quest.goal.upgradeType == QuestGoalType.Kill))
        //    {
        //        playerAcceptedQuest.currentValue++;
        //    }
        //}

        //public void UpdateKillQuest(Monster monster)
        //{
        //    var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
        //    foreach (var playerAcceptedQuest in playerQuests.Where(playerAcceptedQuest => playerAcceptedQuest.quest.goal.upgradeType == QuestGoalType.Kill))
        //    {
        //        playerAcceptedQuest.currentValue++;
        //    }
        //}

        //public void UpdateGatherQuest()
        //{
        //    var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
        //    foreach (var playerAcceptedQuest in playerQuests.Where(playerAcceptedQuest => playerAcceptedQuest.quest.goal.upgradeType == QuestGoalType.Gather))
        //    {
        //        playerAcceptedQuest.currentValue++;
        //    }
        //}
    }
}