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
            if (_timer >= attackTime)
            {
                stateController.player.playerData.temporaryTurnSmoothTime =
                    stateController.player.playerData.turnSmoothTime;
                //TODO: Update these
                if(stateController.player.animator!=null) stateController.player.animator.SetBool("Attack", false);
            }
            else
            {
                stateController.player.playerData.temporaryTurnSmoothTime =
                    stateController.player.playerData.turnSmoothTime * 8;
            }

            _timer += Time.deltaTime;
            if (!stateController.player.inputHandler.attackInput) { return; }

            _timer = 0;
            stateController.player.inputHandler.attackInput = false;
            Vector3 faceTo = Vector3.zero;
            RaycastHit hit;
            Ray ray = stateController.player.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.SphereCast(ray, 1f, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    faceTo = hit.transform.position;
                    _target = hit.transform;
                    stateController.player.target = _target;
                }
                else
                {
                    _target = null;
                    faceTo = hit.point;
                }
            }

            stateController.player.skillManager.skillPoint = faceTo;
            
            if (stateController.player.skillManager.targeting) { return; }
            stateController.transform.LookAt(faceTo);
            if (stateController.player.inputHandler.playerSwitch && _target!=null)
            {
                stateController.player.ReleaseTameBeam();
            }
            stateController.transform.rotation = new Quaternion(0f,stateController.transform.rotation.y, 0f, stateController.transform.rotation.w);
            
            if(stateController.player.animator!=null) stateController.player.animator.SetBool("Attack", true);
        }
    }
}
