using _Core.Managers;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
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
            var playerInputHandler = stateController.player.inputHandler;
            var dashInput = playerInputHandler.dashInput;
            
            if (dashInput && playerInputHandler.currentMonster < 0)
            {
                GameManager.instance.pooler.SpawnFromPool(null, stateController.player.dashGraphic.name,
                    stateController.player.dashGraphic, stateController.transform.position,
                    stateController.transform.rotation);
            }
            return dashInput;
        }
    }
}