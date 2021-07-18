using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Attack Action")]
    public class PlayerAttackAction : Action
    {
        private Transform _target;
        private float _timer;
        public override void Act(StateController stateController)
        {
            Attack(stateController);
        }

        private void Attack(StateController stateController)
        {
            _timer += Time.deltaTime;
            HandleTurnSmoothTime(stateController);
            
            if (!stateController.player.inputHandler.attackInput) { return; }
            stateController.player.inputHandler.attackInput = false;
            
            if (stateController.player.selectionManager.selectables.Count > 0)
            {
                _target = stateController.player.selectionManager.selectables[0];
            }
            var faceTo = stateController.player.selectionManager.selectablePosition;
            stateController.player.skillManager.skillPoint = faceTo;
            var distance = Vector3.Distance(stateController.transform.position,
                stateController.player.selectionManager.selectablePosition);
            if (stateController.player.inputHandler.currentMonster < 0)
            {
                PlayerReleaseTameBeam(stateController, faceTo, distance);
                return;
            }


            var currentMonster = stateController.player.inputHandler.currentMonster;
            if(distance > stateController.player.monsterSlots[currentMonster].monster.basicAttackSkill.castRadius) return;
            
            if (stateController.player.skillManager.targeting) { return; }
            
            _timer = 0;
            MonsterAttack(stateController, faceTo);
        }

        private void MonsterAttack(StateController stateController, Vector3 faceTo)
        {
            stateController.transform.LookAt(faceTo);
            stateController.transform.rotation = new Quaternion(0f, stateController.transform.rotation.y, 0f, stateController.transform.rotation.w);
            stateController.player.ReleaseBasicAttack();
            
            if (stateController.player.currentAnimator != null)
            {
                stateController.player.currentAnimator.SetBool("Attack", true);
            }
        }

        private void PlayerReleaseTameBeam(StateController stateController, Vector3 faceTo, float distance)
        {
            if (_target == null || !(distance <= stateController.player.tameRadius)) return;
            
            _timer = 0;
            stateController.transform.LookAt(faceTo);
            stateController.transform.rotation = new Quaternion(0f, stateController.transform.rotation.y, 0f, stateController.transform.rotation.w);
            stateController.player.ReleaseTameBeam();
            stateController.controllerAnimator.SetBool("Attack", true);
        }

        private void HandleTurnSmoothTime(StateController stateController)
        {
            if (_timer >= stateController.player.tempAttackRate)
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
