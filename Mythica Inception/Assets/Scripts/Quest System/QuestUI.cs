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
                    OpenQuestInfoPanel(acceptedQuests[index].quest, acceptedQuests[index].questGiver.facePicture);
                _questIconItems[i].SetupQuestIcon(acceptedQuests[i].quest, OpenQuest);
            }
            catch
            {
                _questIconItems[i].DisableQuestIcon();
            }
        }
    }

    public void OpenQuestInfoPanel(Quest quest, Sprite questGiverPicture)
    {
        _questTitle.text = quest.title;
        _questDescription.text = quest.description;
        _questGiverImage.sprite = questGiverPicture;
        var rewardsCount = _questRewards.Count;
        
        for (var i = 0; i < rewardsCount; i++)
        {
            _questRewards[i].DisableReward();
            _questRewards[i].gameObject.SetActive(true);
            try
            {
                _questRewards[i].SetupRewardsUI(quest.rewards[i].value.ToString(), quest.rewards[i].rewardsType.icon);
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
