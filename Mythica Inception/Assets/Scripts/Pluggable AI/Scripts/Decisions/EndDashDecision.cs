using _Core.Player;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/End Dash Decision")]
    public class EndDashDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            var dashTime = stateController.player.inputHandler.currentMonster < 0 ? stateController.player.playerData.dashTime : 0;

            var dashTimeDone = stateController.HasTimeElapsed(dashTime);
            stateController.player.inputHandler.dashInput = !dashTimeDone;
            
            if (dashTimeDone && stateController.player.inputHandler.currentMonster < 0)
            {
                StopDash(stateController.player.rgdbody);
            }
            
            return !stateController.player.inputHandler.dashInput;
        }

        private void StopDash(Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.Sleep();
            rigidbody.WakeUp();
        }
    }
}