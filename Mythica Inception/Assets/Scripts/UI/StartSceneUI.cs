using System;
using _Core.Managers;
using _Core.Others;
using Pluggable_AI.Scripts.States;
using ToolBox.Serialization;
using UnityEngine;

namespace UI
{
    public class StartSceneUI : MonoBehaviour
    {
        public ScenePicker scenePicker;
        public GameObject continueButton;
        [SerializeField] private State gameplayState;
        public PlayerSaveData[] playerSavedData = new PlayerSaveData[5];
        private void Start()
        {
            if(GameManager.instance == null) return;
            FillUpSaveFiles();
        }

        public void FillUpSaveFiles()
        {
            for (var i = 0; i < 5; i++)
            {
                if (!DataSerializer.TryLoadProfile<PlayerSaveData>(i, GameManager.instance.saveManager.playerSaveKey,
                        out var playerData))
                {
                    var saveFile = GameManager.instance.uiManager.newGamePanel.saveFiles[i];
                    saveFile.playerSaveData = null;
                    saveFile.playerName.text = string.Empty;
                    saveFile.saveFileInfo.text = string.Empty;
                    saveFile.trashButton.SetActive(false);
                    continue;
                }
                
                continueButton.SetActive(true);
                playerSavedData[i] = playerData;
                GameManager.instance.uiManager.newGamePanel.saveFiles[i].SetSaveFileData(playerData);
            }
        }

        public void ContinueButtonSelected(bool selected)
        {
            GameManager.instance.uiManager.newGamePanel.continueSelected = selected;
        }

        public void NewGameContinueFunctionality()
        {
            var newGamePanel = GameManager.instance.uiManager.newGamePanel;
            if (!newGamePanel.gameObject.activeInHierarchy)
                newGamePanel.gameObject.SetActive(true);

            if (!newGamePanel.mainSavePanel.activeInHierarchy)
            {
                newGamePanel.mainSavePanel.SetActive(true);
            }
            else
            {
                newGamePanel.mainSavePanel.SetActive(false);
                newGamePanel.mainSavePanel.SetActive(true);
            }
        }

        public void Options()
        {
        
        }

        public void ContinueGame(int profileIndex)
        {
            if (GameManager.instance == null) return;

            if (playerSavedData[profileIndex] == null)
            {
                Debug.Log("SavePlayerData File " + profileIndex + " has no saved data. Please create a new save data in SavePlayerData File " + profileIndex + ".");
                return;
            }

            GameManager.instance.uiManager.DeactivateAllUI();
            GameManager.instance.loadedSaveData = playerSavedData[profileIndex];

            var sceneManager = GameManager.instance.gameSceneManager;
            var savedScenePath = playerSavedData[profileIndex].currentScenePath;

            sceneManager.LoadScene(savedScenePath, true);
            sceneManager.UnloadScene(GameManager.instance.currentWorldScenePath, false);
            sceneManager.LoadScene(GameManager.instance.gameplayScene.path, false);
            
            GameManager.instance.uiManager.minimapCamera.SetActive(true);
            GameManager.instance.currentWorldScenePath = savedScenePath;
            GameManager.instance.saveManager.profileIndex = profileIndex;

            //TODO: transition state depending on the situation
            GameManager.instance.gameStateController.TransitionToState(gameplayState);
        }
    }
}
