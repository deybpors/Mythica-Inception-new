using Pluggable_AI.Scripts.Actions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Actions/Move To Waypoint")]
public class MoveToWaypoint : Action
{
    public bool randomWaypoint = true;

    public override void Act(StateController stateController)
    {
        Move(stateController);
    }

    private void Move(StateController stateController)
    {
        if(stateController.aI.waypointCount <= 0) return;

        var nextDestination = stateController.aI.waypoints[stateController.aI.nextWaypoint].position;
        stateController.aI.agent.destination = nextDestination;
        stateController.machineDestination = nextDestination;

        stateController.aI.agent.isStopped = false;

        if (!(stateController.aI.agent.remainingDistance <= stateController.aI.agent.stoppingDistance) ||
            stateController.aI.agent.pathPending) return;
        
        if (randomWaypoint)
        {
            var currentWaypoint = stateController.aI.nextWaypoint;
            do
            {
                stateController.aI.nextWaypoint = Random.Range(0, stateController.aI.waypointCount);
            } while (currentWaypoint == stateController.aI.nextWaypoint);

            return;
        }
        else
        {
            stateController.aI.nextWaypoint = (stateController.aI.nextWaypoint + 1) % stateController.aI.waypointCount;
        }
    }
}
