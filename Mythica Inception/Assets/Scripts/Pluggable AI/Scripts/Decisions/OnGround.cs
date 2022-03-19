using Pluggable_AI.Scripts.Decisions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Player FSM/Decisions/On Ground Decision")]
public class OnGround : Decision
{
    public override bool Decide(StateController stateController)
    {
        return IsControllerOnGround(stateController);
    }

    private bool IsControllerOnGround(StateController stateController)
    {
        var distanceToGround = stateController.player.colliderExtents.y;
        return Physics.Raycast(stateController.transform.position, -Vector3.up, distanceToGround + 0.1f);
    }
}
