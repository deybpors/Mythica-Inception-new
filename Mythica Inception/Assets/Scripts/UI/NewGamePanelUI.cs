using System;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Assets.Scripts._Core.Player;
using Monster_System;
using MyBox;
using Quest_System;
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
    public CharacterSelectionUI selectionUi;
    [HideInInspector] public SaveFileUI saveFileSelected = null;

    [Header("Create New SavePlayerData File")]
    public GameObject newSaveFilePanel;
    public Sprite verifyIcon;
    public TMP_InputField nameInputField;
    public Button maleButton;
    public Button femaleButton;
    [ReadOnly] public SceneReference startPlacePath;
    private Sex selectedSex = Sex.Male;
    private UITweener _newSaveFilePanelTweener;

    [ReadOnly] public bool continueSelected;
    private Color _white = Color.white;

    void Start()
    {
        maleButton.onClick.AddListener(() => PickSexType(maleButton));
        femaleButton.onClick.AddListener(() => PickSexType(femaleButton));
        savePanelMinimize.onClick.AddListener(DisableCreateNewSavePanel);
        try
        {
            //TODO: Change StartSceneUI Scene Picker Data to env_Dream
            startPlacePath = GameManager.instance.uiManager.startSceneUI.startScene;
        }
        catch
        {
            //ignored
        }
    }

    public void ShowTweener(UITweener tweener)
    {
        tweener.gameObject.SetActive(true);
    }

    public void ShowNewSaveFilePanel()
    {
        newSaveFilePanel.SetActive(true);
        nameInputField.text = string.Empty;
        var texture = (RenderTexture) selectionUi.rawImage.texture;
        texture.Release();
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

        var newOptionsData = new OptionsSaveData(true, OptionsSaveData.DifficultyOptions.Dynamic, true, 1, 1, 1, 1);

        var newPlayerData = new PlayerSaveData(nameInputField.text,
            selectedSex, 
            null, 
            GameSettings.GetDefaultMonsterSlots(4),
            new EntityHealth(saveManager.defaultPlayerHealth, saveManager.defaultPlayerHealth),
            GameSettings.GetDefaultInventorySlots(30),
            startPlacePath.sceneIndex,
            new Dictionary<string, Monster>(),
            new Dictionary<string, PlayerAcceptedQuest>(),
            new Dictionary<string, PlayerAcceptedQuest>(),
            TimeSpan.Zero,
            DateTime.Now,
            newOptionsData,
            GameSettings.GetDefaultMonsterSlots(20)
            );
        selectionUi.Disable();

        if (!GameManager.instance.uiManager.startSceneUI.stateObserverCreated)
        {
            DataSerializer.CreateObserver();
            GameManager.instance.uiManager.startSceneUI.stateObserverCreated = true;
        }
        DataSerializer.Setup(saveFileSelected.buttonNum);
        DataSerializer.SaveToProfileIndex(saveFileSelected.buttonNum, saveManager.playerSaveKey, newPlayerData);
        GameManager.instance.uiManager.startSceneUI.playerSavedData[saveFileSelected.buttonNum] = newPlayerData;
        GameManager.instance.uiManager.startSceneUI.ContinueGame(saveFileSelected.buttonNum);
    }

    public void VerifyNewFile()
    {
        if(nameInputField.text == string.Empty) return;

        var playerName = nameInputField.text;
        playerName = playerName.Replace(" ", string.Empty).ToLowerInvariant();
        playerName = char.ToUpperInvariant(playerName[0]) + playerName.Substring(1);
        nameInputField.text = playerName;
        if (selectedSex.Equals(Sex.Male))
        {
            playerName += ", ♂";
        }
        else
        {
            playerName += ", ♀";
        }

        GameManager.instance.uiManager.modal.OpenModal(playerName, verifyIcon, _white, InitializeNewSaveFile);
    }

    private void PickSexType(Button whatButton)
    {
        selectedSex = whatButton.Equals(maleButton) ? Sex.Male : Sex.Female;
    }

    private void DisableCreateNewSavePanel()
    {
        if (continueSelected) return;
        
        if (_newSaveFilePanelTweener == null)
        {
            _newSaveFilePanelTweener = newSaveFilePanel.GetComponent<UITweener>();
        }

        if (!_newSaveFilePanelTweener.disabled)
        {
            _newSaveFilePanelTweener.Disable();
        }
    }
}