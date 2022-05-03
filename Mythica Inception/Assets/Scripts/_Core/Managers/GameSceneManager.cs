using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBox;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public UIManager uiManager;
        public bool loadingScreenOn = true;
        private Dictionary<int, AsyncOperation> _scenesLoading = new Dictionary<int, AsyncOperation>();
        private Dictionary<AsyncOperation, int> _scenes = new Dictionary<AsyncOperation, int>();

        void Update()
        {
            if(!loadingScreenOn) return;

            if (_scenesLoading.Count > 0)
            {
                uiManager.loadingScreen.progressBar.currentValue = 1f / _scenesLoading.Count;
            }
            else
            {
                uiManager.loadingScreen.progressBar.currentValue = 1f;
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(false);
                uiManager.loadingScreen.gameObject.SetActive(false);
                loadingScreenOn = false;
            }
        }

        public void LoadScene(int sceneToAdd, bool withLoadingScreen)
        {
            uiManager.loadingScreen.progressBar.currentValue = 0;

            var scene = SceneManager.LoadSceneAsync(sceneToAdd, LoadSceneMode.Additive);
            _scenesLoading.Add(sceneToAdd, scene);
            _scenes.Add(scene, sceneToAdd);
            scene.completed += (operation => RemoveScene(scene));
            
            scene.allowSceneActivation = true;

            if (withLoadingScreen && !uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(true);
                uiManager.loadingScreen.gameObject.SetActive(true);
                loadingScreenOn = true;
            }
        }

        public void UnloadScene(int loadedScene, bool withLoadingScreen)
        {
            uiManager.loadingScreen.progressBar.currentValue = 0;

            var scene = SceneManager.UnloadSceneAsync(loadedScene);
            try
            {
                _scenesLoading.Add(loadedScene, scene);
                _scenes.Add(scene, loadedScene);
                scene.completed += (operation => RemoveScene(scene));
            }
            catch
            {
                //ignored
            }
            scene.allowSceneActivation = true;

            if (withLoadingScreen && !uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.loadingScreenCamera.gameObject.SetActive(true);
                uiManager.loadingScreen.gameObject.SetActive(true);
                loadingScreenOn = true;
            }
        }

        private void RemoveScene(AsyncOperation scene)
        {
            if (_scenes.TryGetValue(scene, out var sceneIndex))
            {
                _scenes.Remove(scene);
                _scenesLoading.Remove(sceneIndex);
            }
        }
    }
}