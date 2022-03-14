using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Player FSM/Actions/SavePlayerData Data Action")]
public class SaveDataAction : Action
{
    public override void Act(StateController stateController)
    {
        if (!stateController.HasTimeElapsed(GameManager.instance.saveManager.saveSeconds)) return;

        if(GameManager.instance.player.SamePosition()) return;
        
        Debug.Log("Saving data...");
        GameManager.instance.uiManager.debugConsole.DisplayLogUI("Saving data...");
        GameManager.instance.saveManager.SavePlayerData(GameManager.instance.player.GetCurrentSaveData());
    }
}
