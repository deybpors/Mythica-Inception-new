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

    void Start()
    {
        if (tooltipTrigger == null)
        {
            tooltipTrigger = GetComponent<TooltipTrigger>();
        }
    }

    public void SetTextActionOnClickButton(UnityAction action, string text)
    {
        _choiceButton.onClick.RemoveAllListeners();
        _choiceButton.onClick.AddListener(action);
        _choiceText.text = text;
    }
}
