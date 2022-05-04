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
                if (killGoal.IsComplete(updated) && !active.completed) OnComplete(active);
            }
        }

        public void UpdateGatherQuest()
        {
            var playerQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();
            var questCount = playerQuests.Count;

            for (var i = 0; i < questCount; i++)
            {
                var active = playerQuests[i];
                if (!(active.quest.goal is GatherGoal gatherGoal)) continue;

                gatherGoal.ItemGathered(active, out var updated);

                //if the gather goal is not complete or active quest is completed
                if (!gatherGoal.IsComplete(updated) || active.completed)
                {
                    active.completed = gatherGoal.IsComplete(updated);
                    continue;
                }

                OnComplete(active);
            }
        }

        private static void OnComplete(PlayerAcceptedQuest active)
        {
            active.completed = true;
            GameManager.instance.uiManager.questUI.UpdateQuestIcons();
            GameManager.instance.audioManager.PlaySFX("Confirmation");
        }
    }
}