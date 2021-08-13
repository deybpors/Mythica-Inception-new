using System;
using _Core.Managers;
using _Core.Others;
using UnityEngine;

namespace UI
{
    public class StartSceneUI : MonoBehaviour
    {
        public ScenePicker scenePicker;
        private void Start()
        {
            if(GameManager.instance == null) return;
            GameManager.instance.gameplayActive = false;
        }

        public void Continue()
        {
            if(GameManager.instance == null) return;
            GameManager.instance.uiManager.DeactivateAllUI();
            
            var manager = GameManager.instance.gameSceneManager;
            manager.AddLoadedScene(scenePicker.path, true, false);
            manager.UnloadLoadedScene(GameManager.instance.currentWorldScenePath, true, false);
            manager.AddLoadedScene(GameManager.instance.gameplayScene.path, true, true);
            GameManager.instance.uiManager.minimapCamera.SetActive(true);
        }

        public void NewGame()
        {
        
        }

        public void Options()
        {
        
        }
    }
}
