using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public override void Act(StateController stateController)
        {
            Patrol(stateController);
        }

        private void Patrol(StateController stateController)
        {
            if (stateController.aI.waypoints.Count <= 0 && stateController.aI is MonsterTamerAI)
            {
                var ai = (MonsterTamerAI)stateController.aI;
                stateController.aI.waypoints = ai.spawner.waypointsList;
            }
            stateController.controllerAnimator.SetBool("Attack", false);
            var nextDestination = stateController.aI.waypoints[stateController.aI.nextWaypoint].position;
            try
            {
                stateController.aI.agent.destination = nextDestination;
                stateController.machineDestination = nextDestination;

                stateController.aI.agent.isStopped = false;

                if (stateController.aI.agent.remainingDistance <= stateController.aI.agent.stoppingDistance &&
                    !stateController.aI.agent.pathPending)
                {
                    stateController.aI.nextWaypoint =
                        (stateController.aI.nextWaypoint + 1) % stateController.aI.waypoints.Count;
                }
            }
            catch
            {
                stateController.aI.agent.Warp(stateController.thisTransform.position);
            }
        }
    }
}
