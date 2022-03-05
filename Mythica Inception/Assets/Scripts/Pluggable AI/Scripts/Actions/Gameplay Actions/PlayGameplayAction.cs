using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay FSM/Actions/Play Gameplay")]
public class PlayGameplayAction : Action
{
    public override void Act(StateController stateController)
    {
        var player = GameManager.instance.player;
        if (player != null && player.currentGameplayTimeScale < 1)
        {
            player.ChangeTimeScaleGameplay(1);
        }
    }
}
