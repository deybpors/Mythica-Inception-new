using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using Quest_System;
using UnityEngine;

public class PlayerQuestManager : MonoBehaviour
{
    public List<PlayerAcceptedQuest> activeQuests;

    public void GiveQuestToPlayer(Quest questGiven)
    {
        if (PlayerHaveQuest(questGiven)) return;

        var newQuest = new PlayerAcceptedQuest(questGiven);
        activeQuests.Add(newQuest);
    }

    private bool PlayerHaveQuest(Quest quest)
    {
        return activeQuests.Any(playerAcceptedQuest => playerAcceptedQuest.quest.ID.Equals(quest.ID));
    }

    public void RemoveQuestToPlayer(Quest questToRemove)
    {
        var count = activeQuests.Count;
        for (var i = 0; i < count; i++)
        {
            if (!activeQuests[i].quest.ID.Equals(questToRemove.ID)) continue;
            activeQuests.RemoveAt(i);
            break;
        }
    }

    public bool IsQuestSucceeded(Quest quest)
    {
        //check whether the passed quest is finished
        return activeQuests.Where(acceptedQuest => acceptedQuest.quest.ID.Equals(quest.ID)).Any(acceptedQuest => acceptedQuest.currentValue >= acceptedQuest.quest.goals.requiredValue);
    }

    public void GetQuestRewards(Quest quest)
    {
        var rewardsCount = quest.rewards.Count;
        for (var i = 0; i < rewardsCount; i++)
        {
            if (quest.rewards[i].rewardsType.rewardType != RewardTypes.items) continue;

            var item = quest.rewards[i].rewardsType.rewardItem;
            var value = quest.rewards[i].value;
            GameManager.instance.player.inventory.AddItemInPlayerInventory(item, value);

            if (item is Gold)
            {
                GameManager.instance.uiManager.UpdateGoldUI();
            }
        }
    }
}
