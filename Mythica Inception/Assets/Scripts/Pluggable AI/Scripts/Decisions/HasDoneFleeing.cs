using System.Collections;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Has Done Fleeing")]
    public class HasDoneFleeing : Decision
    {
        private IEnumerator _destination;
        
        public override bool Decide(StateController stateController)
        {
            _destination = TravelOppositeToTarget(stateController);
            return DoneFleeing(stateController);
        }
        private bool DoneFleeing(StateController stateController)
        {
            stateController.StartCoroutine(_destination);
            
            return stateController.HasTimeElapsed(stateController.aI.aiData.fleeDuration);
        }

        IEnumerator TravelOppositeToTarget(StateController stateController)
        {
            Vector3 currentPosition = stateController.transform.position;
            Vector3 newDestination = currentPosition - stateController.aI.lastKnownTargetPosition;
            newDestination += currentPosition;
            stateController.aI.agent.destination = newDestination;
            stateController.machineDestination = newDestination;
            yield return new WaitForSeconds(stateController.aI.aiData.fleeDuration);
        }
    }
}
