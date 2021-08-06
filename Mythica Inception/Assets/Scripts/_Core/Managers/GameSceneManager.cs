using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Core.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public UIManager uiManager;
        [HideInInspector] public UITweener loadingUITweener;
        private float _totalSceneProgress;
        private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();
        private Coroutine _disabling;
        public void UnloadCurrentWorldLoadNext(string nextWorldScene, bool withLoadingScreen)
        {
            if (!uiManager.loadingScreen.activeInHierarchy && withLoadingScreen)
            {
                if (!uiManager.loadingScreenCamera.isActiveAndEnabled)
                {
                    uiManager.loadingScreenCamera.gameObject.SetActive(true);
                }
                uiManager.loadingScreen.SetActive(true);
            }
            var worldScene = GameManager.instance.currentWorldScenePath;
            if (!worldScene.Equals(nextWorldScene))
            {
                _scenesLoading.Add(SceneManager.UnloadSceneAsync(worldScene));
            }
            
            _scenesLoading.Add(SceneManager.LoadSceneAsync(nextWorldScene, LoadSceneMode.Additive));
            GameManager.instance.currentWorldScenePath = nextWorldScene;
            StartCoroutine(GetSceneLoadProgress());
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

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        public AsyncOperation AddLoadedScene(string addedScene, bool withLoadingScreen)
        {
            if (!uiManager.loadingScreen.activeInHierarchy && withLoadingScreen)
            {
                if (!uiManager.loadingScreenCamera.isActiveAndEnabled)
                {
                    uiManager.loadingScreenCamera.gameObject.SetActive(true);
                }
                uiManager.loadingScreen.SetActive(true);
            }
            var op = SceneManager.LoadSceneAsync(addedScene, LoadSceneMode.Additive);
            _scenesLoading.Add(op);
            StartCoroutine(GetSceneLoadProgress());
            return op;
        }
    
        private IEnumerator GetSceneLoadProgress()
        {
            var loadingIsActive = false;
            foreach (var operation in _scenesLoading.ToList())
            {
                while (!operation.isDone)
                {
                    _totalSceneProgress = 0;

                    foreach (var scene in _scenesLoading)
                    {
                        _totalSceneProgress += scene.progress;
                    }

                    _totalSceneProgress /= _scenesLoading.Count;
                    if (uiManager.loadingBar.isActiveAndEnabled)
                    {
                        loadingIsActive = true;
                        uiManager.loadingBar.currentValue = _totalSceneProgress;
                    }
                    yield return null;
                }
            }

            if (loadingIsActive && _disabling == null)
            {
                _disabling = StartCoroutine(DisableLoadingScreen());
            }
            _scenesLoading.Clear();
        }

        private IEnumerator DisableLoadingScreen()
        {
            yield return new WaitForSeconds(.05f);
            loadingUITweener.Disable();
        }
    }
}

