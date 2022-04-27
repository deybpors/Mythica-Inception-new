using Monster_System;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Chase")]
    public class ChaseAction : Action
    {
        private float origStopDistance = 0;

        public override void Act(StateController stateController)
        {
            if (origStopDistance == 0)
            {
                origStopDistance = stateController.aI.agent.stoppingDistance;
            }

            Chase(stateController);
        }

        private void Chase(StateController stateController)
        {
            var fieldOfView = stateController.aI.fieldOfView;

            if (fieldOfView.visibleTargets.Count > 0)
            {
                Quaternion transRotation;
                var targetPosition = stateController.aI.target.position;
                var stateControllerTransform = stateController.transform;
                var aiPosition = stateControllerTransform.position;
                transRotation = targetPosition - aiPosition != Vector3.zero ? 
                    Quaternion.LookRotation(targetPosition - aiPosition) : Quaternion.identity;
                
                Quaternion look = new Quaternion(0f,transRotation.y, 0f, transRotation.w);
                stateControllerTransform.rotation = Quaternion.Lerp(stateControllerTransform.rotation, look, .25f);

                if (stateController.aI is MonsterTamerAI monsterTamer)
                {
                    var monster = monsterTamer.monsterSlots[monsterTamer.currentMonster].monster;
                    var isRange = monster.basicAttackType == BasicAttackType.Range;
                    if (isRange)
                    {
                        stateController.aI.agent.stoppingDistance = monster.basicAttackSkill.castRadius;
                    }
                }
                else
                {
                    stateController.aI.agent.stoppingDistance = origStopDistance;
                    stateController.aI.agent.destination = targetPosition;
                    stateController.machineDestination = targetPosition;
                    stateController.aI.lastKnownTargetPosition = targetPosition;
                    stateController.aI.agent.isStopped = false;
                }
            }
            else
            {
                stateController.aI.agent.destination = stateController.aI.lastKnownTargetPosition;
                stateController.machineDestination = stateController.aI.lastKnownTargetPosition;
            }
        }
    }
}
