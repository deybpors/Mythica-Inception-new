using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/Movement Decision")]
    public class MovementDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return ToMove(stateController);
        }

        private bool ToMove(StateController stateController)
        {
            if (stateController.player.inputHandler == null)
            {
                return false;
            }

            bool onGround = CheckGround();
            
            
            //player input
            return stateController.player.inputHandler.movementInput != Vector2.zero;
        }

        private bool CheckGround()
        {
            return false;
        }
    }
}
