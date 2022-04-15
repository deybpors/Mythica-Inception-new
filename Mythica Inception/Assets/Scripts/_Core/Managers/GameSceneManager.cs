using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBox;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public UIManager uiManager;
        private float _target;
        private Dictionary<int, AsyncOperation> _scenesLoading = new Dictionary<int, AsyncOperation>();

        void Update()
        {
            uiManager.loadingScreen.progressBar.currentValue = _target;
            if (!uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(false);
            }
        }

        public async void LoadScene(int sceneToAdd, bool withLoadingScreen)
        {
            _target = 0;
            uiManager.loadingScreen.progressBar.currentValue = 0;

            var scene = SceneManager.LoadSceneAsync(sceneToAdd, LoadSceneMode.Additive);
            _scenesLoading.Add(sceneToAdd, scene);
            scene.allowSceneActivation = false;

            if (withLoadingScreen && !uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(true);
                uiManager.loadingScreen.gameObject.SetActive(true);
            }

            do
            {
                await Task.Delay(100);

                if (!withLoadingScreen) continue;
                _target = GetSceneLoadProgress();
            }
            while (scene.progress < .9f);

            scene.allowSceneActivation = true;

            if (!GetSceneLoadProgress().Approximately(1f) || !withLoadingScreen) return;

            uiManager.loadingScreen.tweener.Disable();
        }

        public async void UnloadScene(int loadedScene, bool withLoadingScreen)
        {
            _target = 0;
            uiManager.loadingScreen.progressBar.currentValue = 0;

            var scene = SceneManager.UnloadSceneAsync(loadedScene);
            _scenesLoading.Add(loadedScene, scene);
            scene.allowSceneActivation = false;

            if (withLoadingScreen && !uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(true);
                uiManager.loadingScreen.gameObject.SetActive(true);
            }
            
            do
            {
                await Task.Delay(100);

                if (!withLoadingScreen) continue;
                _target = GetSceneLoadProgress();
            }
            while (scene.progress < .9f);

            scene.allowSceneActivation = true;

            if (!GetSceneLoadProgress().Approximately(1f) || !withLoadingScreen) return;
            
            uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(false);
        }

        private float GetSceneLoadProgress()
        {
            var sceneCount = _scenesLoading.Count;
            var sceneDone = 0f;
            var currentScenesLoading = _scenesLoading.Values.ToList();

            for (var i = 0; i < sceneCount; i++)
            {
                var scene = currentScenesLoading[i];
                sceneDone += scene.progress;
            }

            return sceneDone / sceneCount;
        }
    }
}