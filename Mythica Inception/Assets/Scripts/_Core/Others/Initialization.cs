using System;
using _Core.Managers;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Others
{
    public class Initialization : MonoBehaviour
    {
        public SceneReference starting;
        private AsyncOperation _managerOp;
        void Awake()
        {
            _managerOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(starting.sceneIndex, LoadSceneMode.Additive);
        }

        private void Update()
        {
            if (!_managerOp.isDone) return;
            GameManager.instance.currentWorldScenePath = starting.sceneIndex;
            GameManager.instance.uiManager.DeactivateAllUI();
            GameManager.instance.uiManager.startSceneUICanvas.SetActive(true);
            GameManager.instance.uiManager.newGamePanel.gameObject.SetActive(true);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
    }
}