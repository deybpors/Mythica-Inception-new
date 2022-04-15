using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay FSM/Actions/Disable Player")]
public class DisablePlayerAction : Action
{
    private Outline _playerOutline;
    public override void Act(StateController stateController)
    {
        GameManager.instance.uiManager.gameplayUICanvas.SetActive(false);

        if (GameManager.instance.player == null) return; 
        _playerOutline = GameManager.instance.player.monsterManager.currentOutline;
        _playerOutline.enabled = false;
    }
}
