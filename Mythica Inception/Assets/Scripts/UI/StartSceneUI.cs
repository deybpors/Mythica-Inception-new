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
            manager.UnloadCurrentWorldLoadNext(scenePicker.path, true);
            manager.AddLoadedScene(GameManager.instance.gameplayScene.path, true);
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
