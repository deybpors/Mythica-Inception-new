using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.States
{
    [CreateAssetMenu(menuName = "Pluggable AI/State")]
    public class State : ScriptableObject
    {
        public Actions.Action[] actions;
        public Transition[] transitions;
        public string stateAnimation;
        public Color gizmoColor = Color.blue;

        public void UpdateState(StateController stateController)
        {
            ExecuteAction(stateController);
            CheckForTransitions(stateController);
        }
        
        private void ExecuteAction(StateController stateController)
        {
            foreach (var action in actions)
            {
                action.Act(stateController);
            }
        }

        private void CheckForTransitions(StateController stateController)
        {
            foreach (var transition in transitions)
            {
                bool decision = transition.decision.Decide(stateController);
                
                if (decision)
                {
                    stateController.TransitionToState(transition.successState);
                }
                else
                {
                    stateController.TransitionToState(transition.failState);
                }
            }
        }
    }
}
