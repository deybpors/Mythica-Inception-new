using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using Quest_System;
using UnityEngine;

public class PlayerQuestManager : MonoBehaviour
{
    public Dictionary<string, PlayerAcceptedQuest> activeQuests = new Dictionary<string, PlayerAcceptedQuest>();
    public Dictionary<string, Quest> finishedQuests = new Dictionary<string, Quest>();

    public void GiveQuestToPlayer(Quest questGiven)
    {
        if (PlayerHaveQuest(questGiven)) return;

        var newQuest = new PlayerAcceptedQuest(questGiven);
        activeQuests.Add(newQuest.quest.ID, newQuest);
    }

    private bool PlayerHaveQuest(Quest quest)
    {
        return activeQuests.TryGetValue(quest.ID, out var playerQuest);
    }

    public void RemoveQuestToPlayer(Quest questToRemove)
    {
        activeQuests.Remove(questToRemove.ID);
    }

    public bool IsQuestSucceeded(Quest quest)
    {
        //check whether the passed quest is finished
        if (activeQuests.TryGetValue(quest.ID, out var playerQuest))
        {
            return playerQuest.currentValue >= playerQuest.quest.goal.requiredValue;
        }

        return false;
    }

    public void GetQuestRewards(Quest quest)
    {
        var rewardsCount = quest.rewards.Count;
        for (var i = 0; i < rewardsCount; i++)
        {
            var questReward = quest.rewards[i];
            switch (questReward.rewardsType.typeOfReward)
            {
                case RewardTypes.Items:
                    var item = questReward.rewardsType.rewardItem;
                    var value = questReward.value;
                    GameManager.instance.player.playerInventory.AddItemInPlayerInventory(item, value);
                    
                    if (item is Gold)
                    {
                        GameManager.instance.uiManager.UpdateGoldUI();
                    }
                    break;
                case RewardTypes.Experience:
                    break;
            }
        }

        try
        {
            finishedQuests.Add(quest.ID, quest);
        }
        catch
        {
            //ignored
        }
    }
}
