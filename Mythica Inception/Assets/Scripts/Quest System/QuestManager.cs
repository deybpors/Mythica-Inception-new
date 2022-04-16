using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using Monster_System;
using Quest_System.Goals;
using UnityEngine;

namespace Quest_System
{
    public class QuestManager : MonoBehaviour
    {
        public void UpdateKillQuest(Monster monster)
        {
            var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
            var questCount = playerQuests.Count;

            for (var i = 0; i < questCount; i++)
            {
                var active = playerQuests[i];
                if (!(active.quest.goal is KillGoal killGoal)) continue;
                
                killGoal.EnemyKilled(active, monster, out var updated);
                active.currentAmount = updated;

                //if the kill goal is complete and active.completed isn't set to true
                if (!killGoal.IsComplete(updated) || active.completed) continue;

                OnComplete(active, playerQuests);
            }
        }

        public void UpdateGatherQuest(InventorySlot item)
        {
            var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
            var questCount = playerQuests.Count;

            for (var i = 0; i < questCount; i++)
            {
                var active = playerQuests[i];
                if (!(active.quest.goal is GatherGoal gatherGoal)) continue;

                gatherGoal.ItemGathered(active, item, out var updated);
                active.currentAmount = updated;

                //if the gather goal is complete and active.completed isn't set to true
                if (!gatherGoal.IsComplete(updated) || active.completed) continue;

                OnComplete(active, playerQuests);
            }
        }

        private static void OnComplete(PlayerAcceptedQuest active, List<PlayerAcceptedQuest> playerQuests)
        {
            active.completed = true;
            GameManager.instance.uiManager.questUI.UpdateQuestIcons(playerQuests);
            GameManager.instance.audioManager.PlaySFX("Confirmation");
        }
    }
}