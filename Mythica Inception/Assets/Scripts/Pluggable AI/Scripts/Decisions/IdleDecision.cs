using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{   
    [CreateAssetMenu(menuName = "Player FSM/Decisions/Idle Decision")]
    public class IdleDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return IdleDecide(stateController);
        }

        private bool IdleDecide(StateController stateController)
        {
            if (stateController.player.inputHandler == null)
            {
                return false;
            }

            bool onGround = CheckGround();
            
            //not moving
            return stateController.player.inputHandler.movementInput == Vector2.zero;
        }
        
        private bool CheckGround()
        {
            return false;
        }
    }
}