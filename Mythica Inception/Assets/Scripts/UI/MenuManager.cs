using Assets.Scripts._Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float skyBoxSpeed;
    public GameObject MainMenu;
    public GameObject QuestMenu;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxSpeed);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainMenu.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                MainMenu.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (QuestMenu.activeSelf)
            {
                QuestMenu.SetActive(false) ;
            }
            else
            {
                QuestMenu.SetActive(true);
            }
        }
    }

    public void Continue()
    {
        GameSceneManager.instance.UnloadCurrentLoadNext((int)GameSceneManager.GameSceneIndexes.TITLE_SCREEN, (int)GameSceneManager.GameSceneIndexes.TEST_SCENE, LoadSceneMode.Additive);
    }

    public void CloseMenu()
    {
        MainMenu.SetActive(false);
    }
}
