using System.Collections;
using System.Collections.Generic;
using UI;
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
        private Coroutine _delayCoroutine;

        void Start()
        {
            uiManager.loadingScreen.Initialize();
        }

        void Update()
        {
            if (!loadingScreenOn) return;

            var scenesCount = _scenesLoading.Count;

            if (scenesCount > 0)
            {
                uiManager.loadingScreen.progressBar.currentValue = 1f / scenesCount;
            }
            else
            {
                if(_delayCoroutine != null) return;
                uiManager.loadingScreen.progressBar.currentValue = 1f;
                if (!uiManager.loadingScreen.tweener.disabled)
                {
                    uiManager.loadingScreen.tweener.Disable();
                }
            }

            loadingScreenOn = uiManager.loadingScreen.thisGameObject.activeInHierarchy;
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
                uiManager.loadingScreen.thisGameObject.SetActive(true);
                loadingScreenOn = true;
            }
        }

        public void LoadScene(int sceneToAdd, bool withLoadingScreen, float delayActivation)
        {
            uiManager.loadingScreen.progressBar.currentValue = 0;

            var scene = SceneManager.LoadSceneAsync(sceneToAdd, LoadSceneMode.Additive);
            _scenesLoading.Add(sceneToAdd, scene);
            _scenes.Add(scene, sceneToAdd);
            scene.completed += (operation => RemoveScene(scene));

            scene.allowSceneActivation = false;

            if (withLoadingScreen && !uiManager.loadingScreen.gameObject.activeInHierarchy)
            {
                uiManager.loadingScreen.thisGameObject.SetActive(true);
                loadingScreenOn = true;
            }

            StopAllCoroutines();
            _delayCoroutine = StartCoroutine(Delay(scene, delayActivation));
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

        private IEnumerator Delay(AsyncOperation scene, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            scene.allowSceneActivation = true;
            _delayCoroutine = null;
        }
    }
}