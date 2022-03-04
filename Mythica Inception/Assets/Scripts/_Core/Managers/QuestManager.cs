using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using UnityEngine;

namespace Quest_System
{
    public class QuestManager : MonoBehaviour
    {
        public Quest questSelected;
        public void UpdateKillQuest()
        {
            var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests;
            foreach (var quest in playerQuests.Where(quest => quest.quest.goals.type == QuestGoalType.kill))
            {
                quest.currentValue++;
            }
        }
        
        public bool QuestAcceptedByPlayer(Quest quest)
        {
            var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests;
            var questCount = playerQuests.Count;
            for (var i = 0; i < questCount; i++)
            {
                if (quest.ID.Equals(playerQuests[i].quest.ID))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}