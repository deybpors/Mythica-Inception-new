using _Core.Managers;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/End Dash Decision")]
    public class EndDashDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            var dashTime = GameManager.instance.inputHandler.currentMonster < 0 ? stateController.player.playerSettings.playerData.dashTime : 0;

            var dashTimeDone = stateController.HasTimeElapsed(dashTime);
            GameManager.instance.inputHandler.dashInput = !dashTimeDone;
            
            if (dashTimeDone && GameManager.instance.inputHandler.currentMonster < 0)
            {
                StopDash(stateController.player.rgdbody);
            }
            
            return !GameManager.instance.inputHandler.dashInput;
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