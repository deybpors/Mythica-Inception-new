using _Core.Managers;
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
            if (GameManager.instance.inputHandler == null)
            {
                return false;
            }

            //not moving
            return GameManager.instance.inputHandler.movementInput == Vector2.zero;
        }
    }
}