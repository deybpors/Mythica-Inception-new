using Quest_System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestIconItemUI : MonoBehaviour
{
    [SerializeField] private TooltipTrigger _tooltipTrigger;
    [SerializeField] private Button _questIconButton;
    [SerializeField] private GameObject _completed;
    private GameObject _thisObject;

    public void SetupQuestIcon(PlayerAcceptedQuest active, UnityAction questIconOnClickAction)
    {
        _tooltipTrigger.SetTitleContent(active.quest.title, active.quest.description);
        _questIconButton.onClick.RemoveAllListeners();
        _questIconButton.onClick.AddListener(questIconOnClickAction);
        _completed.SetActive(active.completed);
    }

    public void DisableQuestIcon()
    {
        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }
        _thisObject.SetActive(false);
    }
}
