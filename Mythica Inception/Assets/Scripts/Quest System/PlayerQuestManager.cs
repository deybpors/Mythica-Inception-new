using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using Quest_System;
using UnityEngine;

public class PlayerQuestManager : MonoBehaviour
{
    public Dictionary<string, PlayerAcceptedQuest> activeQuests = new Dictionary<string, PlayerAcceptedQuest>();
    public Dictionary<string, PlayerAcceptedQuest> finishedQuests = new Dictionary<string, PlayerAcceptedQuest>();

    public PlayerAcceptedQuest GiveQuestToPlayer(Quest questGiven, Character questGiver)
    {
        if (questGiven == null) return null;
        GameManager.instance.uiManager.questUI.UpdateQuestIcons();
        if (PlayerHaveQuest(activeQuests, questGiven, out var accepted)) return null;

        var newQuest = new PlayerAcceptedQuest(questGiven, questGiver, DateTime.Now);
        activeQuests.Add(newQuest.quest.ID, newQuest);
        GameManager.instance.audioManager.PlaySFX("Confirmation");
        GameManager.instance.uiManager.questUI.UpdateQuestIcons();
        return newQuest;
    }

    public Dictionary<string, PlayerAcceptedQuest> GetTotalQuests()
    {
        if (activeQuests != null && finishedQuests != null)
        {
            return activeQuests.Concat(finishedQuests.Where(x => !activeQuests.ContainsKey(x.Key))). //merge all quests where no duplicate keys
                OrderBy(x => x.Value.dateAccepted). //order by date accepted
                ToDictionary(x => x.Key, x => x.Value); //convert to dictionary
        }
        else if (activeQuests != null && finishedQuests == null)
        {
            return activeQuests;
        }
        else if (activeQuests == null && finishedQuests != null)
        {
            return finishedQuests;
        }

        return null;
    }

    public bool PlayerHaveQuest(Dictionary<string, PlayerAcceptedQuest> questList, Quest quest, out PlayerAcceptedQuest accepted)
    {
        accepted = null;
        var haveQuest = questList != null && questList.TryGetValue(quest.ID, out accepted);
        return haveQuest;
    }

    public void RemoveQuestToPlayerAcceptedQuest(Quest questToRemove)
    {
        activeQuests.Remove(questToRemove.ID);
        GameManager.instance.uiManager.questUI.UpdateQuestIcons();
    }

    public void GetQuestRewards(PlayerAcceptedQuest acceptedQuest)
    {
        var rewardsCount = acceptedQuest.quest.rewards.Count;
        for (var i = 0; i < rewardsCount; i++)
        {
            var questReward = acceptedQuest.quest.rewards[i];
            switch (questReward.rewardsType.typeEnumOfReward)
            {
                case RewardTypesEnum.Items:
                    var item = questReward.rewardsType.rewardItem;
                    var value = questReward.value;
                    GameManager.instance.player.playerInventory.AddItemInPlayerInventory(item, value);
                    
                    if (item is Gold)
                    {
                        GameManager.instance.uiManager.UpdateGoldUI();
                    }
                    break;
                case RewardTypesEnum.Experience:
                    GameManager.instance.player.monsterManager.AddMonstersExp(questReward.value);
                    break;
            }
        }

        try
        {
            finishedQuests.Add(acceptedQuest.quest.ID, acceptedQuest);
        }
        catch
        {
            //ignored
        }
    }
}
