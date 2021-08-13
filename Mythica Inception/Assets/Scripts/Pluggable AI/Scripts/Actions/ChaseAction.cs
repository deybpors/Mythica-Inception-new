using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Chase")]
    public class ChaseAction : Action
    {
        public override void Act(StateController stateController)
        {
            Chase(stateController);
        }

        private void Chase(StateController stateController)
        {
            FieldOfView fieldOfView = stateController.aI.fieldOfView;

            if (fieldOfView.visibleTargets.Count > 0)
            {
                Quaternion transRotation;
                var targetPosition = stateController.aI.target.position;
                var aiPosition = stateController.transform.position;
                transRotation = targetPosition - aiPosition != Vector3.zero ? 
                    Quaternion.LookRotation(targetPosition - aiPosition) : Quaternion.identity;
                
                Quaternion look = new Quaternion(0f,transRotation.y, 0f, transRotation.w);
                stateController.transform.rotation = Quaternion.Lerp(stateController.transform.rotation, look, .25f);
                
                stateController.aI.agent.destination = targetPosition;
                stateController.machineDestination = targetPosition;
                stateController.aI.lastKnownTargetPosition = targetPosition;
                stateController.aI.agent.isStopped = false;
            }
            else
            {
                stateController.aI.agent.destination = stateController.aI.lastKnownTargetPosition;
                stateController.machineDestination = stateController.aI.lastKnownTargetPosition;
            }
        }
    }
}
