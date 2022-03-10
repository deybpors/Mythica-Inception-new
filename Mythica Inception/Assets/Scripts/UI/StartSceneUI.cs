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
        public string[] saveKey;
        public GameObject continueButton;
        [SerializeField] private State gameplayState;
        [HideInInspector] public PlayerSaveData[] playerSavedData = new PlayerSaveData[5];
        private void Start()
        {
            if(GameManager.instance == null) return;
            GameManager.instance.gameplayActive = false;

            FillUpSaveFiles();
        }

        private void FillUpSaveFiles()
        {
            for (var i = 0; i < 5; i++)
            {
                if (DataSerializer.TryLoadProfile<PlayerSaveData>(i, "playerData", out var playerData))
                {
                    continueButton.SetActive(true);
                    playerSavedData[i] = playerData;
                    GameManager.instance.uiManager.newGamePanel.saveFiles[i].SetSaveFileData(playerData);
                }
            }
        }

        public void ContinueButtonSelected(bool selected)
        {
            GameManager.instance.uiManager.newGamePanel.continueSelected = selected;
        }

        public void NewGame()
        {
            var newGamePanel = GameManager.instance.uiManager.newGamePanel;
            if (!newGamePanel.gameObject.activeInHierarchy)
                newGamePanel.gameObject.SetActive(true);
            newGamePanel.mainSavePanel.SetActive(true);
            ContinueButtonSelected(false);
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

            var sceneManager = GameManager.instance.gameSceneManager;
            var savedScenePath = playerSavedData[profileIndex].currentScenePath;

            sceneManager.AddLoadedScene(savedScenePath, true, false);
            sceneManager.UnloadLoadedScene(GameManager.instance.currentWorldScenePath, false, false);
            sceneManager.AddLoadedScene(GameManager.instance.gameplayScene.path, false, true);
            GameManager.instance.uiManager.minimapCamera.SetActive(true);
            GameManager.instance.gameStateController.TransitionToState(gameplayState);
            GameManager.instance.currentWorldScenePath = savedScenePath;
            GameManager.instance.loadedSaveData = playerSavedData[profileIndex];
            GameManager.instance.saveManager.profileIndex = profileIndex;
        }
    }
}
