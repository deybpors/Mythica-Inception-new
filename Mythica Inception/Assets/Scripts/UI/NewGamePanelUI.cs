using System;
using _Core.Managers;
using _Core.Others;
using TMPro;
using ToolBox.Serialization;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanelUI : MonoBehaviour
{
    [Header("Main SavePlayerData Panel")] 
    public GameObject mainSavePanel;
    public Button savePanelMinimize;
    public SaveFileUI[] saveFiles;
    public UITweener savePanelTweener;
    [HideInInspector] public SaveFileUI saveFileSelected = null;

    [Header("Create New SavePlayerData File")]
    public GameObject newSaveFilePanel;
    public Sprite verifyIcon;
    public TMP_InputField nameInputField;
    public Button maleButton;
    public Button femaleButton;
    public string startPlacePath;
    private Sex selectedSex = Sex.Male;

    [HideInInspector] public bool continueSelected;

    void Start()
    {
        maleButton.onClick.AddListener(() => PickSexType(maleButton));
        femaleButton.onClick.AddListener(() => PickSexType(femaleButton));
        startPlacePath = GameManager.instance.uiManager.startSceneUI.scenePicker.path;
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
        if (saveFileSelected.playerSaveData != null)
        {
            Debug.Log("SavePlayerData File " + saveFileSelected.buttonNum + " has saved data. It will overwrite.");
        }

        var saveManager = GameManager.instance.saveManager;
        PlayerSaveData newPlayerData = new PlayerSaveData(nameInputField.text, selectedSex, null,
            GameSettings.GetDefaultMonsterSlots(4), new EntityHealth(saveManager.defaultPlayerHealth, saveManager.defaultPlayerHealth), GameSettings.GetDefaultInventorySlots(30), startPlacePath, DateTime.Now, DateTime.Now);
        DataSerializer.SaveToProfileIndex(saveFileSelected.buttonNum, saveManager.playerSaveKey, newPlayerData);
        GameManager.instance.uiManager.startSceneUI.playerSavedData[saveFileSelected.buttonNum] = newPlayerData;
        GameManager.instance.uiManager.startSceneUI.ContinueGame(saveFileSelected.buttonNum);
    }

    public void VerifyNewFile()
    {
        if(nameInputField.text == string.Empty) return;

        var message = nameInputField.text;
        message = message.Replace(" ", string.Empty).ToLowerInvariant();
        message = char.ToUpperInvariant(message[0]) + message.Substring(1);
        nameInputField.text = message;
        if (selectedSex.Equals(Sex.Male))
        {
            message += ", ♂";
        }
        else
        {
            message += ", ♀";
        }

        GameManager.instance.uiManager.modal.OpenModal(message, verifyIcon, InitializeNewSaveFile);
    }

    private void PickSexType(Button whatButton)
    {
        selectedSex = whatButton.Equals(maleButton) ? Sex.Male : Sex.Female;
    }
}