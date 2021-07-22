using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Gravity Apply")]
    public class GravityApply : Action
    {
        public float gravity;
        private Vector3 _gravityVector;
        public override void Act(StateController stateController)
        {
            ApplyGravity(stateController);
        }

        private void ApplyGravity(StateController stateController)
        {
            _gravityVector.y += gravity;
            stateController.player.controller.Move(_gravityVector * Time.deltaTime);
            _gravityVector = Vector3.zero;
        }
    }
}