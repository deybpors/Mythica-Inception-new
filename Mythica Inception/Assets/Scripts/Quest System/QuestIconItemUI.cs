using Quest_System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestIconItemUI : MonoBehaviour
{
    [SerializeField] private TooltipTrigger _tooltipTrigger;
    [SerializeField] private Button _questIconButton;

    public void SetupQuestIcon(Quest quest, UnityAction questIconOnClickAction)
    {
        _tooltipTrigger.title = quest.title;
        _tooltipTrigger.content = quest.description;
        _questIconButton.onClick.RemoveAllListeners();
        _questIconButton.onClick.AddListener(questIconOnClickAction);
    }

    public void DisableQuestIcon()
    {
        gameObject.SetActive(false);
    }
}
