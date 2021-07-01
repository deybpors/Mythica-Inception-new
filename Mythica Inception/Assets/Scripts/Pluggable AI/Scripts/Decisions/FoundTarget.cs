using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Found Targets")]
    public class FoundTarget : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return Look(stateController);
        }

        private bool Look(StateController stateController)
        {
            FieldOfView fieldOfView = stateController.aI.fieldOfView;

            if (fieldOfView.visibleTargets.Count > 0)
            {
                stateController.aI.target = fieldOfView.visibleTargets[0];
                return true;
            }
            
            return false;
        }
    }
}
