using _Core.Managers;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Took Damage")]
    public class IsHit : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return IsAiHit(stateController);
        }

        private bool IsAiHit(StateController stateController)
        {
            bool isHit = stateController.GetComponent<Health>().tookHit;
            return isHit;
        }
    }
}
