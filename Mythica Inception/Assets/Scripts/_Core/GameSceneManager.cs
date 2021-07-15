using System.Collections;
using System.Collections.Generic;
using Assets.Scripts._Core;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public enum GameSceneIndexes
    {
        TITLE_SCREEN = 1,
        TEST_SCENE = 2,
        GAME_SCENE = 3
    }
    
    public static GameSceneManager instance;
    public GameObject loadingScreen;
    public Camera loadingScreenCamera;
    public ProgressBarUI loadingBar;
    private UITweener _tweener;
    private float _totalSceneProgress;
    private float _totalInstantiateProgress;
    
    void Awake()
    {
        instance = this;
        _tweener = loadingScreen.GetComponent<UITweener>();
        SceneManager.LoadSceneAsync((int)GameSceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
    }

    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void UnloadCurrentLoadNext(int currentScene, int nextScene, LoadSceneMode mode)
    {
        loadingScreenCamera.gameObject.SetActive(true);
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync(nextScene, mode));
        StartCoroutine(GetSceneLoadProgress());
        //StartCoroutine(GetTotalProgress());
    }
    
    private IEnumerator GetSceneLoadProgress()
    {
        foreach (var operation in scenesLoading)
        {
            while (!operation.isDone)
            {
                _totalSceneProgress = 0;

                foreach (var scene in scenesLoading)
                {
                    _totalSceneProgress += scene.progress;
                }

                _totalSceneProgress = _totalSceneProgress / scenesLoading.Count;

                yield return null;
            }
        }
        
        loadingScreenCamera.gameObject.SetActive(false);
        _tweener.Disable();
        scenesLoading.Clear();
    }

    public IEnumerator GetTotalProgress()
    {
        while (GameManager.instance == null || !GameManager.instance.pooler.isDone)
        {
            if (GameManager.instance == null)
            {
                _totalInstantiateProgress = 0;
                yield return null;
            }
            else
            {
                _totalInstantiateProgress = (float)GameManager.instance.pooler.objectInstatiated/GameManager.instance.pooler.totalObjectToInstantiate;
            }
            
            loadingBar.currentValue = (_totalSceneProgress + _totalInstantiateProgress) / 2;
        }
        
        loadingScreenCamera.gameObject.SetActive(false);
        _tweener.Disable();
        yield return null;
    }
}

