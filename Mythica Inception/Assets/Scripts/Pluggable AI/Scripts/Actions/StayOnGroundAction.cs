using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Player FSM/Actions/Stay on Ground Action")]
public class StayOnGroundAction : Action
{
    public LayerMask groundLayer;

    private readonly Vector3 _zero = Vector3.zero;
    private readonly Vector3 _down = Vector3.down;
    public override void Act(StateController stateController)
    {
        StayOnGround(stateController);
    }

    private void StayOnGround(StateController stateController)
    {
        var player = stateController.player;
        var onGround = Physics.Raycast(stateController.transform.position, _down, .25f, groundLayer);

        if (onGround)
        {
            player.rgdbody.velocity = _zero;
            player.rgdbody.isKinematic = true;
        }
        else
        {
            stateController.transform.Translate(_down * Time.deltaTime);
        }
    }
}
