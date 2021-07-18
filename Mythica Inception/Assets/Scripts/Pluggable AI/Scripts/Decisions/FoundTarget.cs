using Assets.Scripts._Core.Managers;
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
            var fieldOfView = stateController.aI.fieldOfView;

            if (fieldOfView.visibleTargets.Count <= 0) return false;
            stateController.aI.target = fieldOfView.visibleTargets[0];
            var count = GameManager.instance.enemiesSeePlayer.Count;
            for (var i = 0; i < count; i++)
            {
                if (stateController.transform == GameManager.instance.enemiesSeePlayer[i])
                {
                    return true;
                }
            }
            GameManager.instance.enemiesSeePlayer.Add(stateController.transform);
            return true;
        }
    }
}
