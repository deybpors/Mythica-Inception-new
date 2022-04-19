using _Core.Managers;
using Pluggable_AI.Scripts.Decisions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Decisions/NPC Talking Decision")]
public class NPCTalking : Decision
{
    private readonly Vector3 _zero = Vector3.zero;
    public override bool Decide(StateController stateController)
    {
        if (GameManager.instance == null) return false;

        var isTalking = GameManager.instance.gameStateController.currentState == GameManager.instance.dialogueState;

        if (!isTalking) return false;
        
        try
        {
            stateController.aI.agent.velocity = _zero;
            stateController.aI.agent.isStopped = true;
        }
        catch
        {
            return true;
        }
        return true;
    }
}
