using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Dash")]
    public class DashAction : Action
    {
        private Vector3 _dashVector;
        public override void Act(StateController stateController)
        {
            Dash(stateController);
        }

        private void Dash(StateController stateController)
        {
            _dashVector = stateController.transform.rotation * Vector3.forward * stateController.player.playerData.speed * (stateController.player.playerData.dashSpeed);
            stateController.player.controller.Move( _dashVector * Time.deltaTime);
        }
    }
}