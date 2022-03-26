using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay FSM/Actions/Play Gameplay")]
public class PlayGameplayAction : Action
{
    public override void Act(StateController stateController)
    {
        var pauseManager = GameManager.instance.pauseManager;
        if (pauseManager.paused)
        {
            pauseManager.PauseGameplay(1);
        }

        if (!GameManager.instance.uiManager.gameplayUICanvas.activeInHierarchy)
        {
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
        }
    }
}
