using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Dash")]
    public class DashAction : Action
    {
        public override void Act(StateController stateController)
        {
            Dash(stateController);
        }

        private void Dash(StateController stateController)
        {
            var player = stateController.player;

            if (player == null) return;

            if(player.inputHandler.currentMonster >= 0) return;
            
            if (!player.inputHandler.dashInput) return;
            
            if (player.rgdbody.isKinematic) player.rgdbody.isKinematic = false;

            var force = stateController.transform.forward * player.playerData.dashSpeed *
                        player.playerData.speed * .1f;
            player.rgdbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}