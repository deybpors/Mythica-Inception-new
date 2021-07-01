using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Patrol")]
    public class PatrolAction : Actions.Action
    {
        public override void Act(StateController stateController)
        {
            Patrol(stateController);
        }

        private void Patrol(StateController stateController)
        {
            Vector3 nextDestination = stateController.aI.waypoints[stateController.aI.nextWaypoint].position;
            stateController.aI.agent.destination = nextDestination;
            stateController.machineDestination = nextDestination;
            
            stateController.aI.agent.isStopped = false; //stateController.Agent.Resume() is obsolete
            
            if (stateController.aI.agent.remainingDistance <= stateController.aI.agent.stoppingDistance &&
                !stateController.aI.agent.pathPending)
            {
                stateController.aI.nextWaypoint = (stateController.aI.nextWaypoint + 1) % stateController.aI.waypoints.Count;
            }
        }
    }
}
