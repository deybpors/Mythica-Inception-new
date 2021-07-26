using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
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

            var pointPosition = stateController.player.selectionManager.selectablePosition;
            var pointDistance = Vector3.Distance(stateController.transform.position,
                pointPosition);
            
            if (stateController.player.inputHandler.currentMonster < 0)
            {
                PlayerReleaseTameBeam(stateController, pointPosition, pointDistance);
                return;
            }

            var currentMonster = stateController.player.inputHandler.currentMonster;
            
            stateController.player.skillManager.skillPoint = pointPosition;
            stateController.player.skillManager.pointDistance = pointDistance;
            if (stateController.player.skillManager.targeting)
            {
                FaceToPoint(stateController, pointPosition);
                return;
            }
            
            if (pointDistance > stateController.player.monsterSlots[currentMonster].monster.basicAttackSkill.castRadius) { return; }
            _timer = 0;
            MonsterAttack(stateController, pointPosition);
        }

        private void MonsterAttack(StateController stateController, Vector3 faceTo)
        {
            FaceToPoint(stateController, faceTo);
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
            FaceToPoint(stateController, faceTo);
            stateController.player.ReleaseTameBeam();
            stateController.controllerAnimator.SetBool("Attack", true);
        }

        private static void FaceToPoint(StateController stateController, Vector3 faceTo)
        {
            Transform transform;
            (transform = stateController.transform).LookAt(faceTo);
            var rotation = transform.rotation;
            rotation = new Quaternion(0f, rotation.y, 0f, rotation.w);
            transform.rotation = rotation;
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
