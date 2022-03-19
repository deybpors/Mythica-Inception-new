using System;
using _Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueChoiceUI : MonoBehaviour
{
    [SerializeField] private Button _choiceButton;
    [SerializeField] private TextMeshProUGUI _choiceText;
    public TooltipTrigger tooltipTrigger;

    public void SetTextActionOnClickButton(UnityAction action, string text)
    {
        _choiceButton.onClick.RemoveAllListeners();
        _choiceButton.onClick.AddListener(action);
        _choiceText.text = text;
    }

    public void SetTooltipTrigger(string title, string content)
    {
        GameManager.instance.uiManager.tooltip.titleField.gameObject.SetActive(true);
        GameManager.instance.uiManager.tooltip.contentField.gameObject.SetActive(true);

        if (title.Equals(string.Empty))
            GameManager.instance.uiManager.tooltip.titleField.gameObject.SetActive(false);
        if(content.Equals(string.Empty))
            GameManager.instance.uiManager.tooltip.contentField.gameObject.SetActive(false);
    }
}
