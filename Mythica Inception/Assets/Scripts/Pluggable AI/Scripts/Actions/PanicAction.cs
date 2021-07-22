using Pluggable_AI.Scripts.General;
using Pluggable_AI.Scripts.States;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Panic")]
    public class PanicAction : Action
    {
        public State searchState;
        public override void Act(StateController stateController)
        {
            Panic(stateController);
        }

        private void Panic(StateController stateController)
        {
            if (!stateController.stateBoolVariable)
            {
                stateController.stateTimeElapsed = stateController.aI.aiData.combatDecisionEvery;
                stateController.stateBoolVariable = true;
            }

            if (stateController.HasTimeElapsed(stateController.aI.aiData.combatDecisionEvery))
            {
                DecideWhatToDo(stateController);
            }
        }

        private void DecideWhatToDo(StateController stateController)
        {
            
        }
    }
}
