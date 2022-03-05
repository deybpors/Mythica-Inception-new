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
        public string saveKey;
        public GameObject continueButton;
        [SerializeField] private State gameplayState;
        private PlayerSaveData playerSavedData;
        private void Start()
        {
            if(GameManager.instance == null) return;
            GameManager.instance.gameplayActive = false;

            if (!DataSerializer.HasKey(saveKey))
            {
                continueButton.SetActive(false);
            }
            else
            {
                playerSavedData = GameManager.instance.saveManager.GetPlayerSaveData(saveKey);
            }
        }

        public void Continue()
        {
            if(GameManager.instance == null) return;
            GameManager.instance.uiManager.DeactivateAllUI();
            
            var sceneManager = GameManager.instance.gameSceneManager;
            var scenePath = playerSavedData == null ? scenePicker.path : playerSavedData.scenePath;
            
            sceneManager.AddLoadedScene(scenePath, true, false);
            sceneManager.UnloadLoadedScene(GameManager.instance.currentWorldScenePath, false, false);
            sceneManager.AddLoadedScene(GameManager.instance.gameplayScene.path, false, true);
            GameManager.instance.uiManager.minimapCamera.SetActive(true);
            GameManager.instance.gameStateController.TransitionToState(gameplayState);

            if (GameManager.instance.player != null)
            {
                GameManager.instance.player.savedData = playerSavedData;
            }
        }

        public void NewGame()
        {
            //TODO: Make new game functionality and erase Continue method call
            Continue();
        }

        public void Options()
        {
        
        }
    }
}
