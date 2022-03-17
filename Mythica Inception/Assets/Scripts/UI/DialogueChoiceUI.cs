using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueChoiceUI : MonoBehaviour
{
    [SerializeField] private Button _choiceButton;
    [SerializeField] private TextMeshProUGUI _choiceText;

    public void SetTextActionOnClickButton(UnityAction action, string text)
    {
        _choiceButton.onClick.RemoveAllListeners();
        _choiceButton.onClick.AddListener(action);
        _choiceText.text = text;
    }
}
