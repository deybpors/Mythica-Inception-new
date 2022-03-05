using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay FSM/Actions/Pause Gameplay")]
public class PauseGameplayAction : Action
{
    [Range(0, .5f)]
    public float pauseTimeScale = .1f;
    public override void Act(StateController stateController)
    {
        GameManager.instance.player.ChangeTimeScaleGameplay(pauseTimeScale);
    }
}
