using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
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
                Quaternion transRotation = Quaternion.LookRotation(stateController.aI.target.position - stateController.transform.position);
                Quaternion look = new Quaternion(0f,transRotation.y, 0f, transRotation.w);
                stateController.transform.rotation = Quaternion.Lerp(stateController.transform.rotation, look, .25f);
                
                stateController.aI.agent.destination = stateController.aI.target.position;
                stateController.machineDestination = stateController.aI.target.position;
                stateController.aI.lastKnownTargetPosition = stateController.aI.target.position;
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
