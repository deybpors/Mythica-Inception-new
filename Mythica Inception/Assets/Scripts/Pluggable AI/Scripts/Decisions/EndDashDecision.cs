using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/End Dash Decision")]
    public class EndDashDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            stateController.player.inputHandler.dashInput = !stateController.HasTimeElapsed(stateController.player.playerData.dashTime);

            return !stateController.player.inputHandler.dashInput;
        }
    }
}