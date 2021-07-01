using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/Start Dash Decision")]
    public class StartDashDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return StartDash(stateController);
        }

        private bool StartDash(StateController stateController)
        {
            return stateController.player.inputHandler.dashInput;
        }
    }
}