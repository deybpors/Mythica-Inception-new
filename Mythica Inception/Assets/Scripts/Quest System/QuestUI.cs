using System;
using System.Collections.Generic;
using System.Linq;
using Quest_System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("Quest Info Panel")] 
    [SerializeField] private UITweener _infoPanelTweener;
    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private TextMeshProUGUI _questDescription;
    [SerializeField] private Image _questGiverImage;
    [SerializeField] private List<QuestRewardsUI> _questRewards;
    [SerializeField] private Sprite _experienceRewardIcon;
    [SerializeField] private Button _acceptButton;

    [Header("Quest Icon Panel")]
    [SerializeField] private List<QuestIconItemUI> _questIconItems;

    public void UpdateQuestIcons(List<PlayerAcceptedQuest> acceptedQuests)
    {
        var iconItemsCount = _questIconItems.Count;
        for (var i = 0; i < iconItemsCount; i++)
        {
            _questIconItems[i].gameObject.SetActive(true);
            try
            {
                var index = i;
                void OpenQuest() =>
                    OpenQuestInfoPanel(acceptedQuests[index], acceptedQuests[index].questGiver.facePicture);
                _questIconItems[i].SetupQuestIcon(acceptedQuests[i], OpenQuest);
            }
            catch
            {
                _questIconItems[i].DisableQuestIcon();
            }
        }
    }

    private void OpenQuestInfoPanel(PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        _questTitle.text = active.quest.title;

        var description = active.quest.description;
        if (active.completed)
        {
            description += "\n\n<align=\"center\"><color=#b3f47a>This quest is completed. You may go to " + active.questGiver.fullName + " to claim your reward.";
        }
        _questDescription.text = description;
        _questGiverImage.sprite = questGiverPicture;
        var rewardsCount = _questRewards.Count;
        
        for (var i = 0; i < rewardsCount; i++)
        {
            _questRewards[i].DisableReward();
            _questRewards[i].gameObject.SetActive(true);
            try
            {
                var icon = active.quest.rewards[i].rewardsType.typeEnumOfReward == RewardTypesEnum.Items
                    ? active.quest.rewards[i].rewardsType
                        .rewardItem.itemIcon
                    : _experienceRewardIcon;
                _questRewards[i].SetupRewardsUI(active.quest.rewards[i].rewardsType.typeEnumOfReward,
                    active.quest.rewards[i].rewardsType.rewardItem, active.quest.rewards[i].value, icon);
            }
            catch
            {
                _questRewards[i].DisableReward();
            }
        }

        _infoPanelTweener.gameObject.SetActive(true);
    }

    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
