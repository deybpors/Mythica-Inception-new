using _Core.Managers;
using MyBox;
using Pluggable_AI.Scripts.States;
using ToolBox.Serialization;
using UnityEngine;

namespace UI
{
    public class StartSceneUI : MonoBehaviour
    {
        public SceneReference startScene;
        public GameObject continueButton;
        public GameObject startPanel;
        public Sprite quitSprite;

        [HideInInspector] public bool stateObserverCreated;

        [SerializeField] private State gameplayState;
        public PlayerSaveData[] playerSavedData = new PlayerSaveData[5];

        void OnEnable()
        {
            if(GameManager.instance == null) return;
            FillUpSaveFiles();
            startPanel.SetActive(true);
        }



        public void FillUpSaveFiles()
        {
            continueButton.SetActive(false);
            for (var i = 0; i < 5; i++)
            {
                if (!DataSerializer.FileExists(i))
                {
                    var saveFile = GameManager.instance.uiManager.newGamePanel.saveFiles[i];
                    saveFile.playerSaveData = null;
                    saveFile.playerName.text = string.Empty;
                    saveFile.saveFileInfo.text = string.Empty;
                    saveFile.trashButton.SetActive(false);
                    continue;
                }

                if (!stateObserverCreated)
                {
                    DataSerializer.CreateObserver();
                    stateObserverCreated = true;
                }

                DataSerializer.Setup(i);
                DataSerializer.TryLoadProfile<PlayerSaveData>(i, GameManager.instance.saveManager.playerSaveKey, out var playerData);

                if (playerData == null)
                {
                    DataSerializer.DeleteProfileIndex(i, GameManager.instance.saveManager.playerSaveKey);
                    i--;
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

        public void Quit()
        {
            GameManager.instance.uiManager.modal.OpenModal("Are you sure you want to quit the game?", quitSprite, Color.white, QuitGame);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                        // Application.Quit() does not work in the editor so
                        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }

        public void ContinueGame(int profileIndex)
        {
            if (GameManager.instance == null) return;

            if (playerSavedData[profileIndex] == null)
            {
                GameManager.instance.audioManager.PlaySFX("Error");
                return;
            }

            GameManager.instance.uiManager.DeactivateAllUI();
            var newGamePanel = GameManager.instance.uiManager.newGamePanel.newSaveFilePanel;
            newGamePanel.SetActive(false);
            GameManager.instance.loadedSaveData = playerSavedData[profileIndex];

            var sceneManager = GameManager.instance.gameSceneManager;
            var savedScenePath = playerSavedData[profileIndex].currentSceneIndex;

            sceneManager.UnloadScene(GameManager.instance.startScene.sceneIndex, true);
            sceneManager.LoadScene(GameManager.instance.gameplayScene.sceneIndex, true, 1f);
            sceneManager.LoadScene(savedScenePath, true);

            GameManager.instance.uiManager.minimapCamera.SetActive(true);
            GameManager.instance.currentWorldScenePath = savedScenePath;
            GameManager.instance.saveManager.profileIndex = profileIndex;

        }
    }
}
