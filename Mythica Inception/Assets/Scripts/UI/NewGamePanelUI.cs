using System;
using _Core.Managers;
using _Core.Others;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanelUI : MonoBehaviour
{
    [Header("Main Save Panel")] 
    public GameObject mainSavePanel;
    public Button savePanelMinimize;
    public SaveFileUI[] saveFiles;
    public UITweener savePanelTweener;

    [Header("Create New Save File")]
    public GameObject newSaveFilePanel;
    public Button newSaveFileMinimize;
    public InputField nameInputField;
    public Button maleButton;
    public Button femaleButton;
    public UITweener newSaveFileTweener;
    private Sex selectedSex = Sex.Male;

    [Header("Verify Modal")] public TextMeshProUGUI info;

    void Start()
    {
        maleButton.onClick.AddListener(() => PickSexType(maleButton));
        femaleButton.onClick.AddListener(() => PickSexType(femaleButton));
    }

    public void ShowTweener(UITweener tweener)
    {
        tweener.gameObject.SetActive(true);
    }

    public void ShowNewSaveFilePanel()
    {
        newSaveFilePanel.SetActive(true);
        nameInputField.text = string.Empty;
    }

    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }

    public void InitializeNewSaveFile()
    {
        //TODO: Initialize new Save file
    }

    public void VerifyInfo()
    {
        info.text = nameInputField.text + ", ";
        if (selectedSex.Equals(Sex.Male))
        {
            info.text += "♂";
        }
        else
        {
            info.text += "♀";
        }
    }

    private void PickSexType(Button whatButton)
    {
        selectedSex = whatButton.Equals(maleButton) ? Sex.Male : Sex.Female;
    }
}