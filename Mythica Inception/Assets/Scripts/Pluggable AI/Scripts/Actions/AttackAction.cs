using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Attack")]
    public class AttackAction : Action
    {
        public override void Act(StateController stateController)
        {
            Attack(stateController);
        }

        private void Attack(StateController stateController)
        {
            FieldOfView fieldOfView = stateController.aI.fieldOfView;

            if (!stateController.stateBoolVariable)
            {
                stateController.stateTimeElapsed = stateController.aI.aiStats.attackDecisionEvery;
                stateController.stateBoolVariable = true;
            }

            if (fieldOfView.visibleTargets.Count > 0)
            {
                if (stateController.HasTimeElapsed(stateController.aI.aiStats.attackDecisionEvery))
                {
                    //TODO: attack here
                }
            }
        }
    }
}
