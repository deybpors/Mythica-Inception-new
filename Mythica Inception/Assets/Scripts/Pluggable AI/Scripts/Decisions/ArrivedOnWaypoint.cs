using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Arrived on Waypoint Decision")]
    public class ArrivedOnWaypoint : Decision
    {
        public override bool Decide(StateController stateController)
        {
            if (stateController.aI.agent == null) return true;

            var arrive = stateController.aI.agent.remainingDistance <= stateController.aI.agent.stoppingDistance &&
                         !stateController.aI.agent.pathPending;
            return arrive;
        }
    }
}