using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public float aiStoppingDistance;

        public override void Act(StateController stateController)
        {
            Patrol(stateController);
        }

        private void Patrol(StateController stateController)
        {
            if (stateController.aI.waypointCount <= 0 && stateController.aI is MonsterTamerAI ai)
            {
                ai.waypoints = ai.spawner.waypointsList;
                ai.waypointCount = ai.waypoints.Count;
            }
            stateController.controllerAnimator.SetBool("Attack", false);
            try
            {
                stateController.aI.agent.stoppingDistance = aiStoppingDistance;

                var nextDestination = stateController.aI.waypoints[stateController.aI.nextWaypoint].position;
                stateController.aI.agent.destination = nextDestination;
                stateController.machineDestination = nextDestination;

                stateController.aI.agent.isStopped = false;
            }
            catch
            {
                stateController.aI.agent.Warp(stateController.thisTransform.position);
            }

            if (stateController.aI.agent.remainingDistance <= stateController.aI.agent.stoppingDistance &&
                !stateController.aI.agent.pathPending)
            {
                stateController.aI.nextWaypoint =
                    (stateController.aI.nextWaypoint + 1) % stateController.aI.waypointCount;
            }
        }
    }
}
