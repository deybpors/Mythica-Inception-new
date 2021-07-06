using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Attack Action")]
    public class PlayerAttackAction : Action
    {
        public float attackTime;
        private Transform _target;
        private float _timer;
        public override void Act(StateController stateController)
        {
            Attack(stateController);
        }

        private void Attack(StateController stateController)
        {
            HandleTurnSmoothTime(stateController);

            _timer += Time.deltaTime;
            if (!stateController.player.inputHandler.attackInput) { return; }
            _timer = 0;
            stateController.player.inputHandler.attackInput = false;

            if (stateController.player.selectionManager.selectables.Count > 0)
            {
                _target = stateController.player.selectionManager.selectables[0];
            }

            var faceTo = stateController.player.selectionManager.selectablePosition;
            stateController.player.skillManager.skillPoint = faceTo;
            if (stateController.player.skillManager.targeting) { return; }
            stateController.transform.LookAt(faceTo);
            if (stateController.player.inputHandler.playerSwitch && _target!=null)
            {
                stateController.player.ReleaseTameBeam();
            }
            //TODO: Request to object pooler for attack here
            stateController.transform.rotation = new Quaternion(0f,stateController.transform.rotation.y, 0f, stateController.transform.rotation.w);
            if(stateController.player.currentAnimator!=null) {stateController.player.currentAnimator.SetBool("Attack", true);}
        }

        private void HandleTurnSmoothTime(StateController stateController)
        {
            if (_timer >= attackTime)
            {
                stateController.player.playerData.temporaryTurnSmoothTime =
                    stateController.player.playerData.turnSmoothTime;
                if (stateController.player.currentAnimator != null)
                {
                    stateController.player.currentAnimator.SetBool("Attack", false);
                }
            }
            else
            {
                stateController.player.playerData.temporaryTurnSmoothTime =
                    stateController.player.playerData.turnSmoothTime * 8;
            }
        }
    }
}
