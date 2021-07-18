using Assets.Scripts._Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float skyBoxSpeed;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxSpeed);
    }

    public void Continue()
    {
        GameSceneManager.instance.UnloadCurrentLoadNext((int)GameSceneManager.GameSceneIndexes.TITLE_SCREEN, (int)GameSceneManager.GameSceneIndexes.TEST_SCENE, LoadSceneMode.Additive);
    }
}
