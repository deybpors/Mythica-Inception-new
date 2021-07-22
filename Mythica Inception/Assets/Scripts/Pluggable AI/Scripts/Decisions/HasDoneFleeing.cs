using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Has Done Fleeing")]
    public class HasDoneFleeing : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return DoneFleeing(stateController);
        }
        private bool DoneFleeing(StateController stateController)
        {
            TravelOppositeToTarget(stateController);
            return stateController.HasTimeElapsed(stateController.aI.aiData.fleeDuration);
        }

        void TravelOppositeToTarget(StateController stateController)
        {
            Vector3 currentPosition = stateController.transform.position;
            Vector3 newDestination = currentPosition - stateController.aI.lastKnownTargetPosition;
            newDestination += currentPosition;
            stateController.aI.agent.destination = newDestination;
            stateController.machineDestination = newDestination;
        }
    }
}
