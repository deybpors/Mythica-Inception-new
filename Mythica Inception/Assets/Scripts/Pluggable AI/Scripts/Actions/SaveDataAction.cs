using _Core.Managers;
using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Player FSM/Actions/SavePlayerData Data Action")]
public class SaveDataAction : Action
{
    public override void Act(StateController stateController)
    {
        if (!GameManager.instance.saveManager.activated) return;

        if (!stateController.HasTimeElapsed(GameManager.instance.saveManager.saveSeconds)) return;

        if(GameManager.instance.player.SamePositionFromSaved()) return;

        if(GameManager.instance.gameStateController.currentState != GameManager.instance.gameplayState) return;

        if(GameManager.instance.loadedSaveData == null || !GameManager.instance.loadedSaveData.optionsSaveData.autoSave) return;

        var data = GameManager.instance.player.GetCurrentSaveData();
        GameManager.instance.saveManager.SavePlayerData(data);
        GameManager.instance.uiManager.debugConsole.DisplayLogUI("Saving data...");
    }
}
