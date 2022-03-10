using System;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Items_and_Barter_System.Scripts;
using Monster_System;
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
    public Button newSaveFileMinimize;
    public TMP_InputField nameInputField;
    public Button maleButton;
    public Button femaleButton;
    public UITweener newSaveFileTweener;
    public string startPlacePath;
    private Sex selectedSex = Sex.Male;

    [HideInInspector] public bool continueSelected;

    [Header("Verify Modal")] public TextMeshProUGUI info;

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

        var playerPosition = new Vector3(48.75f, 10.51f, -148.57f);
        PlayerSaveData newPlayerData = new PlayerSaveData(nameInputField.text, selectedSex, playerPosition,
            GameSettings.GetDefaultMonsterSlots(4), new EntityHealth(0, 0), GameSettings.GetDefaultInventorySlots(30), startPlacePath, DateTime.Now, DateTime.Now);
        DataSerializer.SaveToProfileIndex(saveFileSelected.buttonNum, GameManager.instance.saveManager.playerSaveKey, newPlayerData);
        GameManager.instance.uiManager.startSceneUI.playerSavedData[saveFileSelected.buttonNum] = newPlayerData;
        GameManager.instance.uiManager.startSceneUI.ContinueGame(saveFileSelected.buttonNum);
    }

    public void VerifyInfo()
    {
        var newName = nameInputField.text;
        newName = newName.Replace(" ", string.Empty).ToLowerInvariant();
        newName = char.ToUpperInvariant(newName[0]) + newName.Substring(1);
        nameInputField.text = newName;
        info.text = newName + ", ";
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