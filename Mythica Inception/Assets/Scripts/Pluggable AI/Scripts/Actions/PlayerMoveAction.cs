using _Core.Player;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Move Action")]
    public class PlayerMoveAction : Action
    {

        private readonly Vector3 _zero = Vector3.zero;
        public override void Act(StateController stateController)
        {
            Move(stateController);
        }

        private void Move(StateController stateController)
        {

            var player = stateController.player;
            
            if (player.rgdbody.isKinematic)
            {
                player.rgdbody.isKinematic = false;
            }

            var inputHandlerMovementInput = player.inputHandler.movementInput;
            
            //get moveVector from input
            var inputVector = new Vector3(inputHandlerMovementInput.x, 0f, inputHandlerMovementInput.y);

            //Move and get Target vector then rotate towards the target vector
            RotateTowardsTargetVector(player, MoveAndGetTargetVector(player, inputVector));
        }

        private void RotateTowardsTargetVector(Player player, Vector3 targetVector)
        {
            if(targetVector == _zero) return;

            var targetRotation = Quaternion.LookRotation(targetVector);
            player.transform.rotation =
                Quaternion.RotateTowards(player.transform.rotation, targetRotation, player.playerData.moveRotationSpeed);
        }

        private Vector3 MoveAndGetTargetVector(Player player, Vector3 inputVector)
        {
            var speed = player.tempSpeed * Time.deltaTime;
            
            var targetVector = Quaternion.Euler(0, player.mainCamera.transform.eulerAngles.y, 0) * inputVector;
            var targetPosition = player.transform.position + targetVector * speed;
            player.transform.position = targetPosition;
            return targetVector;
        }
    }
}