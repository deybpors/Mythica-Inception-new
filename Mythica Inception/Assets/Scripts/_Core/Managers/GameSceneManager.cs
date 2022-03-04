using System.Collections.Generic;
using MyBox;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public UIManager uiManager;
        [HideInInspector] public UITweener loadingUITweener;
        private float _totalSceneProgress = 0f;
        private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();
        private bool loading = false;
        private float _prevSceneProgress = 0f;

        private void Update()
        {
            GetSceneLoadProgress();
        }


        public void UnloadCurrentWorldLoadNext(string nextWorldScene, bool withLoadingScreen, bool startLoadProgress)
        {
            if (!uiManager.loadingScreen.activeInHierarchy && withLoadingScreen)
            {
                if (!uiManager.loadingScreenCamera.isActiveAndEnabled)
                {
                    uiManager.loadingScreenCamera.gameObject.SetActive(true);
                }
                uiManager.loadingScreen.SetActive(true);
            }

            var currentWorldScene = GameManager.instance.currentWorldScenePath;

            if (!currentWorldScene.Equals(nextWorldScene))
            {
                _scenesLoading.Add(SceneManager.UnloadSceneAsync(currentWorldScene));
                _scenesLoading.Add(SceneManager.LoadSceneAsync(nextWorldScene, LoadSceneMode.Additive));
                GameManager.instance.currentWorldScenePath = nextWorldScene;
            }

            if (startLoadProgress && !loading)
            {
                loading = true;
            }
        }

        public void UnloadAllScenesExcept(string sceneName)
        {
            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name != sceneName)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        public AsyncOperation AddLoadedScene(string addedScene, bool withLoadingScreen, bool startLoadProgress)
        {
            if (!uiManager.loadingScreen.activeInHierarchy && withLoadingScreen)
            {
                if (!uiManager.loadingScreenCamera.isActiveAndEnabled)
                {
                    uiManager.loadingScreenCamera.gameObject.SetActive(true);
                }
                uiManager.loadingScreen.SetActive(true);
            }

            var operation = SceneManager.LoadSceneAsync(addedScene, LoadSceneMode.Additive);
            _scenesLoading.Add(operation);

            if (startLoadProgress && !loading)
            {
                loading = true;
            }

            return operation;
        }
        
        public AsyncOperation UnloadLoadedScene(string loadedScene, bool withLoadingScreen, bool startLoadProgress)
        {
            if (!uiManager.loadingScreen.activeInHierarchy && withLoadingScreen)
            {
                if (!uiManager.loadingScreenCamera.isActiveAndEnabled)
                {
                    uiManager.loadingScreenCamera.gameObject.SetActive(true);
                }
                uiManager.loadingScreen.SetActive(true);
            }

            var operation = SceneManager.UnloadSceneAsync(loadedScene);
            _scenesLoading.Add(operation);

            if (startLoadProgress && !loading)
            {
                loading = true;
            }

            return operation;
        }

        private void GetSceneLoadProgress()
        {
            if(!loading) return;
            
            var sceneCount = _scenesLoading.Count;
            //Debug.Log(sceneCount);

            var sceneDone = 0;

            for (var i = 0; i < sceneCount; i++)
            {
                var operation = _scenesLoading[i];
                if (operation.isDone)
                {
                    sceneDone++;
                }
            }
            
            //Debug.Log(sceneDone);
            
            _prevSceneProgress = _totalSceneProgress;
            _totalSceneProgress = (float) sceneDone / sceneCount;

            //Debug.Log(_prevSceneProgress);
            //Debug.Log(_totalSceneProgress);
            
            if (_totalSceneProgress.Approximately(_prevSceneProgress))
            {
                uiManager.loadingBar.maxValue = 1f;
                uiManager.loadingBar.currentValue = _totalSceneProgress;
            }

            if (sceneDone != sceneCount) return;

            loadingUITweener.Disable();
            loading = false;
            _scenesLoading.Clear();
        }
    }
}