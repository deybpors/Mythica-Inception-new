using System.Linq;
using _Core.Managers;
using MyBox;
using UnityEngine;

[CreateAssetMenu(menuName = "Others/Go To World Scene")]
public class GoToWorldScene : ScriptableObject
{
    public SceneReference sceneToGo;
    public bool loadDelay;
    [ConditionalField(nameof(loadDelay))] public float loadDelayValue;

    public void GoToWorld()
    {
        var current = GameManager.instance.currentWorldScenePath;
        var toGo = sceneToGo.sceneIndex;

        var activeEnemies = GameManager.instance.activeEnemies.Values.ToList();
        var activeEnemiesCount = activeEnemies.Count;
        for (var i = 0; i < activeEnemiesCount; i++)
        {
            activeEnemies[i].SetActive(false);
            GameManager.instance.RemoveInActiveEnemies(activeEnemies[i]);
        }


        var sceneManager = GameManager.instance.gameSceneManager;
        sceneManager.UnloadScene(current, true);

        if (loadDelay)
        {
            sceneManager.LoadScene(toGo, true, loadDelayValue);
        }
        else
        {
            sceneManager.LoadScene(toGo, false);
        }
        GameManager.instance.currentWorldScenePath = toGo;
        GameManager.instance.inputHandler.EnterGameplay();
    }
}
