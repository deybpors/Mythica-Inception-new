using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay FSM/Actions/Pause Gameplay")]
public class PauseGameplayAction : Action
{
    public override void Act(StateController stateController)
    {
        GameManager.instance.pauseManager.PauseGameplay(GameManager.instance.player.playerSettings.pauseTimeScale);
    }
}
