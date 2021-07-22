using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Move Action")]
    public class PlayerMoveAction : Action
    {
        private Vector3 _moveVector;
        private float _turnSmoothVelocity;
        public override void Act(StateController stateController)
        {
            Move(stateController);
        }

        private void Move(StateController stateController)
        {
            _moveVector = new Vector3(stateController.player.inputHandler.movementInput.x, 0f,
                stateController.player.inputHandler.movementInput.y);
            
            if (!(_moveVector.magnitude >= 0.1f)) return;
            
            float targetAngle = Mathf.Atan2(_moveVector.x, _moveVector.z) * Mathf.Rad2Deg + stateController.player.mainCamera.transform.eulerAngles.y;
            float newAngle = Mathf.SmoothDampAngle(stateController.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                stateController.player.playerData.temporaryTurnSmoothTime);
            stateController.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);
            _moveVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            stateController.player.controller.Move(_moveVector.normalized * stateController.player.tempSpeed * Time.deltaTime);
            
        }
    }
}